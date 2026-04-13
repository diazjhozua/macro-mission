import { type FoodResult } from "@/lib/types/food";

interface Props {
  food: FoodResult;
}

// Compact per-100g macro summary shown in table rows and search results.
export function FoodMacrosBadges({ food }: Props) {
  return (
    <div className="flex gap-2 text-xs text-muted-foreground tabular-nums">
      <span className="text-orange-600 font-medium">{Math.round(food.calories)} kcal</span>
      <span>P {Math.round(food.protein)}g</span>
      <span>C {Math.round(food.carbs)}g</span>
      <span>F {Math.round(food.fat)}g</span>
    </div>
  );
}
