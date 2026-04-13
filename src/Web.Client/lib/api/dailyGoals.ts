import { apiClient } from "@/lib/api/client";
import { type CreateDailyGoalRequest, type DailyGoalResult, type UpdateDailyGoalRequest } from "@/lib/types/dailyGoal";

export async function getDailyGoals(): Promise<DailyGoalResult[]> {
  const { data } = await apiClient.get<DailyGoalResult[]>("/api/v1/dailygoals");
  return data;
}

export async function createDailyGoal(payload: CreateDailyGoalRequest): Promise<DailyGoalResult> {
  const { data } = await apiClient.post<DailyGoalResult>("/api/v1/dailygoals", payload);
  return data;
}

export async function updateDailyGoal(id: string, payload: UpdateDailyGoalRequest): Promise<DailyGoalResult> {
  const { data } = await apiClient.put<DailyGoalResult>(`/api/v1/dailygoals/${id}`, payload);
  return data;
}

export async function deleteDailyGoal(id: string): Promise<void> {
  await apiClient.delete(`/api/v1/dailygoals/${id}`);
}
