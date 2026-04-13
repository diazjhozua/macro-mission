"use client";

import { useQuery } from "@tanstack/react-query";
import { searchFoods } from "@/lib/api/foods";

export const foodKeys = {
  search: (term: string) => ["foods", "search", term] as const,
};

export function useSearchFoods(term: string) {
  return useQuery({
    queryKey: foodKeys.search(term),
    queryFn: () => searchFoods(term),
    // Don't fire until the user has typed at least 1 character.
    // An empty search is valid on the backend but returns everything —
    // better to wait for intent.
    enabled: term.length > 0,
    staleTime: 1000 * 60, // food list rarely changes mid-session
  });
}
