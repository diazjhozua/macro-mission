"use client";

import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { createFood, deleteFood, searchFoods, updateFood } from "@/lib/api/foods";
import { type CreateFoodRequest, type UpdateFoodRequest } from "@/lib/types/food";

export const foodKeys = {
  search: (term: string) => ["foods", "search", term] as const,
};

// alwaysFire lets the foods page load results on mount with an empty term.
// The meal log combobox keeps the default (only fires after the user types).
export function useSearchFoods(term: string, alwaysFire = false) {
  return useQuery({
    queryKey: foodKeys.search(term),
    queryFn: () => searchFoods(term),
    enabled: alwaysFire || term.length > 0,
    staleTime: 1000 * 60,
  });
}

function useFoodInvalidation() {
  const queryClient = useQueryClient();
  // Invalidate all search variants — term could be anything the user typed.
  return () => queryClient.invalidateQueries({ queryKey: ["foods", "search"] });
}

export function useCreateFood() {
  const invalidate = useFoodInvalidation();
  return useMutation({
    mutationFn: (payload: CreateFoodRequest) => createFood(payload),
    onSuccess: invalidate,
  });
}

export function useUpdateFood() {
  const invalidate = useFoodInvalidation();
  return useMutation({
    mutationFn: ({ id, payload }: { id: string; payload: UpdateFoodRequest }) =>
      updateFood(id, payload),
    onSuccess: invalidate,
  });
}

export function useDeleteFood() {
  const invalidate = useFoodInvalidation();
  return useMutation({
    mutationFn: (id: string) => deleteFood(id),
    onSuccess: invalidate,
  });
}
