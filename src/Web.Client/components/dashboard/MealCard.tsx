import { Card, CardContent, CardHeader } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { type MealResult } from "@/lib/types/meal";

interface Props {
  meal: MealResult;
}

const MEAL_TYPE_COLORS: Record<string, string> = {
  Breakfast: "bg-amber-50 text-amber-700 border-amber-200",
  Lunch:     "bg-green-50 text-green-700 border-green-200",
  Dinner:    "bg-blue-50 text-blue-700 border-blue-200",
  Snack:     "bg-purple-50 text-purple-700 border-purple-200",
};

export function MealCard({ meal }: Props) {
  return (
    <Card>
      <CardHeader className="pb-3 pt-4 px-5">
        <div className="flex items-center justify-between">
          <Badge
            variant="outline"
            className={MEAL_TYPE_COLORS[meal.mealType]}
          >
            {meal.mealType}
          </Badge>
          <span className="text-sm font-semibold tabular-nums">
            {Math.round(meal.totals.calories)} kcal
          </span>
        </div>
      </CardHeader>

      <CardContent className="px-5 pb-4 space-y-1.5">
        {meal.items.map((item, i) => (
          <div key={i} className="flex items-center justify-between text-sm">
            <span className="text-foreground">{item.foodName}</span>
            <div className="flex items-center gap-3 text-muted-foreground tabular-nums shrink-0 ml-4">
              <span>{item.grams}g</span>
              <span className="w-16 text-right">{Math.round(item.macros.calories)} kcal</span>
            </div>
          </div>
        ))}

        {/* Macro summary row */}
        <div className="flex gap-4 pt-2 border-t text-xs text-muted-foreground tabular-nums">
          <span>P {Math.round(meal.totals.protein)}g</span>
          <span>C {Math.round(meal.totals.carbs)}g</span>
          <span>F {Math.round(meal.totals.fat)}g</span>
          <span>Fiber {Math.round(meal.totals.fiber)}g</span>
        </div>
      </CardContent>
    </Card>
  );
}
