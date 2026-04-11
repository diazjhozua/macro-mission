"use client";

import { useQuery } from "@tanstack/react-query";
import { getCurrentUser } from "@/lib/api/social";
import { useAuthStore } from "@/lib/stores/authStore";

export const currentUserKey = ["currentUser"] as const;

export function useCurrentUser() {
  const isAuthenticated = useAuthStore((s) => s.isAuthenticated());

  return useQuery({
    queryKey: currentUserKey,
    queryFn: getCurrentUser,
    // Only fetch once the access token is in the store — avoids a 401
    // on first render before AuthProvider has finished the silent refresh.
    enabled: isAuthenticated,
    // User profile rarely changes — no need to refetch on window focus.
    staleTime: 1000 * 60 * 5,
  });
}
