type MacroKey = "calories" | "protein" | "carbs" | "fat" | "fiber";

// Cap at 100 so the progress bar never overflows the container.
export function calcPercent(consumed: number, goal: number): number {
  if (goal <= 0) return 0;
  return Math.min(Math.round((consumed / goal) * 100), 100);
}

// Tailwind indicator colors per macro — consistent across progress bars and labels.
export const MACRO_COLORS: Record<MacroKey, string> = {
  calories: "bg-orange-500",
  protein:  "bg-blue-500",
  carbs:    "bg-yellow-500",
  fat:      "bg-rose-500",
  fiber:    "bg-green-500",
};

export const MACRO_LABELS: Record<MacroKey, string> = {
  calories: "Calories",
  protein:  "Protein",
  carbs:    "Carbs",
  fat:      "Fat",
  fiber:    "Fiber",
};

// Unit label per macro — calories are kcal, the rest are grams.
export const MACRO_UNITS: Record<MacroKey, string> = {
  calories: "kcal",
  protein:  "g",
  carbs:    "g",
  fat:      "g",
  fiber:    "g",
};

export const MACRO_KEYS: MacroKey[] = ["calories", "protein", "carbs", "fat", "fiber"];
