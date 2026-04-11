import axios, { AxiosError, type InternalAxiosRequestConfig } from "axios";
import { ApiError, type ProblemDetails, type ValidationProblemDetails } from "@/lib/types/api";
import { type AuthResponse } from "@/lib/types/auth";
import { useAuthStore } from "@/lib/stores/authStore";

const BASE_URL = process.env.NEXT_PUBLIC_API_URL;

export const apiClient = axios.create({
  baseURL: BASE_URL,
  headers: { "Content-Type": "application/json" },
});

// ─── Shared refresh ───────────────────────────────────────────────────────────

// A single in-flight promise shared across ALL callers (AuthProvider, the
// Axios interceptor, etc.). If two things try to refresh at the same time —
// e.g. AuthProvider on page load AND a 401 from a query that fired before
// the token was ready — only one fetch goes out. Both callers await the same
// result, so the backend never sees the rotated token replayed.
let refreshPromise: Promise<string> | null = null;

export async function refreshAccessToken(): Promise<string> {
  if (refreshPromise) return refreshPromise;

  refreshPromise = (async () => {
    const res = await fetch("/api/auth/refresh", { method: "POST" });

    if (!res.ok) {
      throw new Error("Refresh failed");
    }

    const data: AuthResponse = await res.json();
    useAuthStore.getState().setTokens(data.accessToken, data.accessTokenExpiresAt);
    return data.accessToken;
  })().finally(() => {
    // Always clear the promise so the next refresh attempt can start fresh.
    refreshPromise = null;
  });

  return refreshPromise;
}

// ─── Token injection ──────────────────────────────────────────────────────────

apiClient.interceptors.request.use((config) => {
  const token = useAuthStore.getState().accessToken;

  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }

  return config;
});

// ─── Silent refresh ───────────────────────────────────────────────────────────

apiClient.interceptors.response.use(
  (response) => response,
  async (error: AxiosError) => {
    const original = error.config as InternalAxiosRequestConfig & { _retry?: boolean };

    // Only attempt a refresh on 401s we haven't already retried.
    if (error.response?.status !== 401 || original._retry) {
      return Promise.reject(toApiError(error));
    }

    original._retry = true;

    try {
      const token = await refreshAccessToken();
      original.headers.Authorization = `Bearer ${token}`;
      return apiClient(original);
    } catch (refreshError) {
      useAuthStore.getState().clear();

      if (typeof window !== "undefined") {
        window.location.href = "/login";
      }

      return Promise.reject(toApiError(refreshError as AxiosError));
    }
  },
);

// ─── Error normalisation ──────────────────────────────────────────────────────

function toApiError(error: AxiosError | unknown): ApiError {
  if (error instanceof AxiosError && error.response) {
    const problem = error.response.data as ProblemDetails | ValidationProblemDetails;
    return new ApiError(error.response.status, problem);
  }

  return new ApiError(0, { title: "Network error", status: 0 });
}
