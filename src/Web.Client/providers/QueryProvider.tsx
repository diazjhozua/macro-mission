"use client";

import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { useState, type ReactNode } from "react";

interface Props {
  children: ReactNode;
}

export function QueryProvider({ children }: Props) {
  // One QueryClient per browser session. Instantiated inside useState so each
  // request gets its own client in SSR — avoids shared state between users.
  const [queryClient] = useState(
    () =>
      new QueryClient({
        defaultOptions: {
          queries: {
            // Don't retry on 401/403 — the interceptor handles re-auth.
            // Retrying would just queue more requests behind a refresh.
            retry: (failureCount, error) => {
              if (error instanceof Error && "status" in error) {
                const status = (error as { status: number }).status;
                if (status === 401 || status === 403) return false;
              }
              return failureCount < 2;
            },
            staleTime: 1000 * 30, // 30 seconds before a query is considered stale
          },
        },
      }),
  );

  return <QueryClientProvider client={queryClient}>{children}</QueryClientProvider>;
}
