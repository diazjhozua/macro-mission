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
      // Tag the error with the status so the interceptor can decide whether
      // to redirect to login (401) or just surface the error (429, 500, etc.)
      const err = new Error("Refresh failed") as Error & { status: number };
      err.status = res.status;
      throw err;
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
      const status = (refreshError as { status?: number }).status;

      // Only force a logout on a genuine auth failure. A rate limit (429) or
      // server error (500) is transient — the user still has a valid session.
      if (status === 401 || status === undefined) {
        useAuthStore.getState().clear();

        if (typeof window !== "undefined") {
          window.location.href = "/login";
        }
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
