"use client";

import { useEffect, useRef, type ReactNode } from "react";
import { refreshAccessToken } from "@/lib/api/client";
import { useAuthStore } from "@/lib/stores/authStore";

interface Props {
  children: ReactNode;
}

// On every cold page load the access token is gone (it lives in memory only).
// This calls the shared refreshAccessToken() to rehydrate from the httpOnly
// cookie. Using the shared function means if a query also fires a refresh at
// the same moment, only one backend call goes out — no token reuse collision.
export function AuthProvider({ children }: Props) {
  const clear = useAuthStore((s) => s.clear);
  const attempted = useRef(false);

  useEffect(() => {
    if (attempted.current) return;
    attempted.current = true;

    refreshAccessToken().catch(() => {
      // No valid session — middleware will redirect on the next navigation.
      clear();
    });
  }, [clear]);

  return <>{children}</>;
}
