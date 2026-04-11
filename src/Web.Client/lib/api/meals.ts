import { apiClient } from "@/lib/api/client";
import { type DailySummary, type MealResult } from "@/lib/types/meal";

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
