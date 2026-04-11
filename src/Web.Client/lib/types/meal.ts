// Must match the backend's MealType enum (serialized as strings globally).
export type MealType = "Breakfast" | "Lunch" | "Dinner" | "Snack";

export interface MacroTotals {
  calories: number;
  protein: number;
  carbs: number;
  fat: number;
  fiber: number;
}

// Macros on each item are computed at write time on the backend:
// grams / 100 * per100gValue — they are snapshotted, not recalculated.
export interface MealItem {
  foodId: string;
  foodName: string;
  grams: number;
  macros: MacroTotals;
}

export interface MealResult {
  id: string;
  mealType: MealType;
  date: string;
  loggedAt: string;
  items: MealItem[];
  totals: MacroTotals;
  createdAt: string;
}

// goal is null when the user hasn't set an active daily goal yet.
export interface DailySummary {
  date: string;
  consumed: MacroTotals;
  goal: MacroTotals | null;
  meals: MealResult[];
}

export interface MealItemRequest {
  foodId: string;
  grams: number;
}

export interface CreateMealRequest {
  mealType: MealType;
  date?: string;
  items: MealItemRequest[];
}

// The backend's update endpoint reuses the same request shape as create.
export type UpdateMealRequest = CreateMealRequest;
