"use client";

import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import {
  createDailyGoal,
  deleteDailyGoal,
  getDailyGoals,
  updateDailyGoal,
} from "@/lib/api/dailyGoals";
import { type CreateDailyGoalRequest, type UpdateDailyGoalRequest } from "@/lib/types/dailyGoal";

export const goalKeys = {
  all: ["dailyGoals"] as const,
};

export function useDailyGoals() {
  return useQuery({
    queryKey: goalKeys.all,
    queryFn: getDailyGoals,
  });
}

function useGoalInvalidation() {
  const queryClient = useQueryClient();
  // Always invalidate all goals — setting one active changes another to inactive,
  // so a partial invalidation would leave stale data on the previously active card.
  return () => queryClient.invalidateQueries({ queryKey: goalKeys.all });
}

export function useCreateDailyGoal() {
  const invalidate = useGoalInvalidation();
  return useMutation({
    mutationFn: (payload: CreateDailyGoalRequest) => createDailyGoal(payload),
    onSuccess: invalidate,
  });
}

export function useUpdateDailyGoal() {
  const invalidate = useGoalInvalidation();
  return useMutation({
    mutationFn: ({ id, payload }: { id: string; payload: UpdateDailyGoalRequest }) =>
      updateDailyGoal(id, payload),
    onSuccess: invalidate,
  });
}

export function useDeleteDailyGoal() {
  const invalidate = useGoalInvalidation();
  return useMutation({
    mutationFn: (id: string) => deleteDailyGoal(id),
    onSuccess: invalidate,
  });
}
