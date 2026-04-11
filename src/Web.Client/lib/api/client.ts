import axios, { AxiosError, type InternalAxiosRequestConfig } from "axios";
import { ApiError, type ProblemDetails, type ValidationProblemDetails } from "@/lib/types/api";
import { type AuthResponse } from "@/lib/types/auth";
// authStore doesn't import from this file so there's no circular dependency.
import { useAuthStore } from "@/lib/stores/authStore";

const BASE_URL = process.env.NEXT_PUBLIC_API_URL;

export const apiClient = axios.create({
  baseURL: BASE_URL,
  headers: { "Content-Type": "application/json" },
});

// ─── Token injection ──────────────────────────────────────────────────────────

apiClient.interceptors.request.use((config) => {
  const token = useAuthStore.getState().accessToken;

  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }

  return config;
});

// ─── Silent refresh ───────────────────────────────────────────────────────────

// When multiple concurrent requests all 401 at once (e.g. token expired
// mid-session), only one refresh call should fire. The rest queue here and
// resolve once the single refresh completes.
let isRefreshing = false;
let queue: Array<{
  resolve: (token: string) => void;
  reject: (err: unknown) => void;
}> = [];

function processQueue(err: unknown, token: string | null) {
  queue.forEach((p) => (err ? p.reject(err) : p.resolve(token!)));
  queue = [];
}

apiClient.interceptors.response.use(
  (response) => response,
  async (error: AxiosError) => {
    const original = error.config as InternalAxiosRequestConfig & { _retry?: boolean };

    // Only attempt a refresh on 401s we haven't already retried.
    if (error.response?.status !== 401 || original._retry) {
      return Promise.reject(toApiError(error));
    }

    if (isRefreshing) {
      return new Promise((resolve, reject) => {
        queue.push({
          resolve: (token) => {
            original.headers.Authorization = `Bearer ${token}`;
            resolve(apiClient(original));
          },
          reject,
        });
      });
    }

    original._retry = true;
    isRefreshing = true;

    try {
      // Use fetch instead of axios so the httpOnly cookie is sent reliably.
      // Axios has known quirks with cookies on same-origin requests in some
      // Next.js/webpack builds; fetch handles it correctly every time.
      const res = await fetch("/api/auth/refresh", { method: "POST" });

      if (!res.ok) {
        throw new Error("Refresh failed");
      }

      const data: AuthResponse = await res.json();
      useAuthStore.getState().setTokens(data.accessToken, data.accessTokenExpiresAt);

      processQueue(null, data.accessToken);
      original.headers.Authorization = `Bearer ${data.accessToken}`;
      return apiClient(original);
    } catch (refreshError) {
      processQueue(refreshError, null);

      useAuthStore.getState().clear();

      if (typeof window !== "undefined") {
        window.location.href = "/login";
      }

      return Promise.reject(toApiError(refreshError as AxiosError));
    } finally {
      isRefreshing = false;
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
