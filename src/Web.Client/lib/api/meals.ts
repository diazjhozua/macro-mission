import { apiClient } from "@/lib/api/client";
import { type CreateMealRequest, type DailySummary, type MealResult, type UpdateMealRequest } from "@/lib/types/meal";

export async function getDailySummary(date: string): Promise<DailySummary> {
  const { data } = await apiClient.get<DailySummary>("/api/v1/meals/summary", {
    params: { date },
  });
  return data;
}

export async function getMealsByDate(date: string): Promise<MealResult[]> {
  const { data } = await apiClient.get<MealResult[]>("/api/v1/meals", {
    params: { date },
  });
  return data;
}

export async function createMeal(payload: CreateMealRequest): Promise<MealResult> {
  const { data } = await apiClient.post<MealResult>("/api/v1/meals", payload);
  return data;
}

export async function updateMeal(id: string, payload: UpdateMealRequest): Promise<MealResult> {
  const { data } = await apiClient.put<MealResult>(`/api/v1/meals/${id}`, payload);
  return data;
}

export async function deleteMeal(id: string): Promise<void> {
  await apiClient.delete(`/api/v1/meals/${id}`);
}
