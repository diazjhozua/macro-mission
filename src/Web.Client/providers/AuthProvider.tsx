"use client";

import { useEffect, useRef, type ReactNode } from "react";
import { useAuthStore } from "@/lib/stores/authStore";
import { type AuthResponse } from "@/lib/types/auth";

interface Props {
  children: ReactNode;
}

// On every cold page load the access token is gone (it lives in memory only).
// This provider attempts a silent refresh on mount to rehydrate it from the
// httpOnly cookie, keeping the user logged in across refreshes and new tabs.
export function AuthProvider({ children }: Props) {
  const setTokens = useAuthStore((s) => s.setTokens);
  const clear = useAuthStore((s) => s.clear);

  // Guard against the effect running twice in React 18 strict mode.
  const attempted = useRef(false);

  useEffect(() => {
    if (attempted.current) return;
    attempted.current = true;

    async function rehydrate() {
      try {
        const res = await fetch("/api/auth/refresh", { method: "POST" });

        if (!res.ok) {
          // No valid cookie (first visit, logout, or token reuse revocation).
          // middleware.ts handles the redirect — nothing to do here.
          clear();
          return;
        }

        const data: AuthResponse = await res.json();
        setTokens(data.accessToken, data.accessTokenExpiresAt);
      } catch {
        // Network failure — leave the store empty; middleware will redirect
        // to /login when the user tries to access a protected route.
        clear();
      }
    }

    rehydrate();
  }, [setTokens, clear]);

  return <>{children}</>;
}
