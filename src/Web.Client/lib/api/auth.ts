import axios from "axios";
import { type AuthResponse } from "@/lib/types/auth";
import { type LoginRequest, type RegisterRequest } from "@/lib/types/auth";
import { ApiError, type ProblemDetails } from "@/lib/types/api";

// Auth calls hit the Next.js route handlers (/api/auth/*), not the backend
// directly. This keeps the refresh token out of browser JS entirely.

async function handleResponse<T>(res: Response): Promise<T> {
  const data = await res.json();
  if (!res.ok) throw new ApiError(res.status, data as ProblemDetails);
  return data as T;
}

export async function login(payload: LoginRequest): Promise<AuthResponse> {
  const res = await fetch("/api/auth/login", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(payload),
  });
  return handleResponse<AuthResponse>(res);
}

export async function register(payload: RegisterRequest): Promise<void> {
  const res = await fetch("/api/auth/register", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(payload),
  });
  await handleResponse<unknown>(res);
}

export async function verifyEmail(token: string): Promise<void> {
  const res = await fetch("/api/auth/verify-email", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ token }),
  });
  await handleResponse<unknown>(res);
}

export async function logout(): Promise<void> {
  // Fire and forget — even if this fails the client-side state is cleared.
  await fetch("/api/auth/logout", { method: "POST" }).catch(() => {});
}
