// Macros are stored per 100g on the backend — the frontend displays them as-is
// on the food search list, and scales them when building meal item previews.
export interface FoodResult {
  id: string;
  name: string;
  brand: string | null;
  isCustom: boolean;
  calories: number;
  protein: number;
  carbs: number;
  fat: number;
  fiber: number;
  createdAt: string;
  updatedAt: string;
}

export interface CreateFoodRequest {
  name: string;
  brand?: string;
  calories: number;
  protein: number;
  carbs: number;
  fat: number;
  fiber: number;
}

// Same shape as create — the backend uses a separate record but the fields
// are identical, so we reuse the type on the frontend.
export type UpdateFoodRequest = CreateFoodRequest;
