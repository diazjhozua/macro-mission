import { X } from "lucide-react";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { type FoodResult } from "@/lib/types/food";

interface Props {
  food: FoodResult;
  grams: number;
  onGramsChange: (grams: number) => void;
  onRemove: () => void;
}

export function MealItemRow({ food, grams, onGramsChange, onRemove }: Props) {
  // Live macro preview scaled from per-100g values.
  const ratio = grams / 100;
  const calories = Math.round(food.calories * ratio);
  const protein  = Math.round(food.protein  * ratio * 10) / 10;

  return (
    <div className="flex items-center gap-3 py-2 border-b last:border-0">
      <div className="flex-1 min-w-0">
        <p className="text-sm font-medium truncate">{food.name}</p>
        <p className="text-xs text-muted-foreground tabular-nums">
          {calories} kcal · {protein}g protein
        </p>
      </div>

      <div className="flex items-center gap-1.5 shrink-0">
        <Input
          type="number"
          min={1}
          value={grams}
          onChange={(e) => onGramsChange(Math.max(1, Number(e.target.value)))}
          className="w-20 h-8 text-sm text-right"
        />
        <span className="text-sm text-muted-foreground">g</span>
      </div>

      <Button
        type="button"
        variant="ghost"
        size="icon"
        className="h-7 w-7 text-muted-foreground hover:text-destructive shrink-0"
        onClick={onRemove}
      >
        <X className="h-3.5 w-3.5" />
      </Button>
    </div>
  );
}
