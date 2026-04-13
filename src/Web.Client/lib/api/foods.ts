import { apiClient } from "@/lib/api/client";
import { type CreateFoodRequest, type FoodResult, type UpdateFoodRequest } from "@/lib/types/food";

export async function searchFoods(term: string, page = 1, pageSize = 20): Promise<FoodResult[]> {
  const { data } = await apiClient.get<FoodResult[]>("/api/v1/foods", {
    params: { term, page, pageSize },
  });
  return data;
}

export async function createFood(payload: CreateFoodRequest): Promise<FoodResult> {
  const { data } = await apiClient.post<FoodResult>("/api/v1/foods", payload);
  return data;
}

export async function updateFood(id: string, payload: UpdateFoodRequest): Promise<FoodResult> {
  const { data } = await apiClient.put<FoodResult>(`/api/v1/foods/${id}`, payload);
  return data;
}

export async function deleteFood(id: string): Promise<void> {
  await apiClient.delete(`/api/v1/foods/${id}`);
}
