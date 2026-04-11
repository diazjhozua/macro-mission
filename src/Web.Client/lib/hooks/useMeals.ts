"use client";

import { useQuery } from "@tanstack/react-query";
import { getDailySummary, getMealsByDate } from "@/lib/api/meals";

export const mealKeys = {
  summary: (date: string) => ["meals", "summary", date] as const,
  byDate:  (date: string) => ["meals", "byDate", date] as const,
};

export function useDailySummary(date: string) {
  return useQuery({
    queryKey: mealKeys.summary(date),
    queryFn: () => getDailySummary(date),
  });
}

export function useMealsByDate(date: string) {
  return useQuery({
    queryKey: mealKeys.byDate(date),
    queryFn: () => getMealsByDate(date),
  });
}
