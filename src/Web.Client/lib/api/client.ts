import axios, { AxiosError, type InternalAxiosRequestConfig } from "axios";
import { ApiError, type ProblemDetails, type ValidationProblemDetails } from "@/lib/types/api";
import { type AuthResponse } from "@/lib/types/auth";

// Pulled from env so the backend URL is never hardcoded.
// Set NEXT_PUBLIC_API_URL=http://localhost:5000 in .env.local.
const BASE_URL = process.env.NEXT_PUBLIC_API_URL;

export const apiClient = axios.create({
  baseURL: BASE_URL,
  headers: { "Content-Type": "application/json" },
});

// ─── Token injection ─────────────────────────────────────────────────────────

// Attach the current access token from the Zustand store to every request.
// Imported lazily inside the interceptor to avoid a circular dep at module
// load time (store → client → store).
apiClient.interceptors.request.use((config) => {
  const { getState } = require("@/lib/stores/authStore").useAuthStore;
  const token: string | null = getState().accessToken;

  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }

  return config;
});

// ─── Silent refresh ───────────────────────────────────────────────────────────

// When multiple concurrent requests all get a 401 (e.g. token expired mid-session),
// we only want one refresh call to fire. Every other request queues here and
// resolves once the single refresh completes.
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

    // Only attempt a refresh on 401s that haven't already been retried.
    // Avoids infinite loops if the refresh itself returns 401.
    if (error.response?.status !== 401 || original._retry) {
      return Promise.reject(toApiError(error));
    }

    if (isRefreshing) {
      // Another request already kicked off a refresh — wait for it.
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
      // The refresh route handler reads the httpOnly cookie automatically —
      // no body needed from us.
      const { data } = await axios.post<AuthResponse>("/api/auth/refresh");

      const { useAuthStore } = require("@/lib/stores/authStore");
      useAuthStore.getState().setTokens(data.accessToken, data.accessTokenExpiresAt);

      processQueue(null, data.accessToken);
      original.headers.Authorization = `Bearer ${data.accessToken}`;
      return apiClient(original);
    } catch (refreshError) {
      processQueue(refreshError, null);

      // Refresh failed (token reuse detected, expired, etc.) — log the user out.
      const { useAuthStore } = require("@/lib/stores/authStore");
      useAuthStore.getState().clear();

      // Redirect to login. Using window.location so this works outside React.
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

// Converts an Axios error into our typed ApiError so callers never have to
// inspect raw Axios internals or guess the response shape.
function toApiError(error: AxiosError | unknown): ApiError {
  if (error instanceof AxiosError && error.response) {
    const problem = error.response.data as ProblemDetails | ValidationProblemDetails;
    return new ApiError(error.response.status, problem);
  }

  // Network errors, timeouts, etc. — no response body.
  return new ApiError(0, { title: "Network error", status: 0 });
}
