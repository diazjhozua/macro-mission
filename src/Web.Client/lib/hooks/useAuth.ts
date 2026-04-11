"use client";

import { useMutation, useQueryClient } from "@tanstack/react-query";
import { useRouter, useSearchParams } from "next/navigation";
import { login, logout, register, verifyEmail } from "@/lib/api/auth";
import { useAuthStore } from "@/lib/stores/authStore";
import { ApiError } from "@/lib/types/api";
import { type LoginRequest, type RegisterRequest } from "@/lib/types/auth";

export function useLogin() {
  const setTokens = useAuthStore((s) => s.setTokens);
  const router = useRouter();
  const searchParams = useSearchParams();

  return useMutation({
    mutationFn: (payload: LoginRequest) => login(payload),
    onSuccess: (data) => {
      setTokens(data.accessToken, data.accessTokenExpiresAt);
      // Respect the ?next= param set by middleware so the user lands back
      // on the page they were trying to reach before being redirected.
      const next = searchParams.get("next") ?? "/dashboard";
      router.push(next);
    },
  });
}

export function useRegister() {
  return useMutation({
    mutationFn: (payload: RegisterRequest) => register(payload),
    // onSuccess is handled in the component — it shows a confirmation message
    // rather than navigating, since the user needs to verify their email first.
  });
}

export function useVerifyEmail() {
  const router = useRouter();

  return useMutation({
    mutationFn: (token: string) => verifyEmail(token),
    onSuccess: () => {
      // Brief delay so the user sees the success state before being redirected.
      setTimeout(() => router.push("/login"), 2000);
    },
  });
}

export function useLogout() {
  const clear = useAuthStore((s) => s.clear);
  const queryClient = useQueryClient();
  const router = useRouter();

  return useMutation({
    mutationFn: logout,
    onSettled: () => {
      // Clear cached server state so stale data isn't shown if another user
      // logs in on the same browser without a full page refresh.
      queryClient.clear();
      clear();
      router.push("/login");
    },
  });
}
