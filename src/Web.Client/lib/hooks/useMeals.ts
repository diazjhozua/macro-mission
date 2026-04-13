"use client";

import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { createMeal, deleteMeal, getMealsByDate, getDailySummary, updateMeal } from "@/lib/api/meals";
import { type CreateMealRequest, type UpdateMealRequest } from "@/lib/types/meal";
import { toApiDate } from "@/lib/utils/date";

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

// Invalidates both the meal list and the summary (which includes macro totals)
// for the relevant date so the dashboard stays in sync.
function useMealInvalidation() {
  const queryClient = useQueryClient();

  return (date: string) => {
    queryClient.invalidateQueries({ queryKey: mealKeys.byDate(date) });
    queryClient.invalidateQueries({ queryKey: mealKeys.summary(date) });
  };
}

export function useCreateMeal() {
  const invalidate = useMealInvalidation();

  return useMutation({
    mutationFn: (payload: CreateMealRequest) => createMeal(payload),
    onSuccess: (_, variables) => {
      invalidate(variables.date ?? toApiDate(new Date()));
    },
  });
}

export function useUpdateMeal(date: string) {
  const invalidate = useMealInvalidation();

  return useMutation({
    mutationFn: ({ id, payload }: { id: string; payload: UpdateMealRequest }) =>
      updateMeal(id, payload),
    onSuccess: () => invalidate(date),
  });
}

export function useDeleteMeal(date: string) {
  const invalidate = useMealInvalidation();

  return useMutation({
    mutationFn: (id: string) => deleteMeal(id),
    onSuccess: () => invalidate(date),
  });
}
