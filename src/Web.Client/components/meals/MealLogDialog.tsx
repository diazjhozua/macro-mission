"use client";

import { useEffect, useState } from "react";
import { toast } from "sonner";
import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { Label } from "@/components/ui/label";
import { FoodSearchCombobox } from "@/components/meals/FoodSearchCombobox";
import { MealItemRow } from "@/components/meals/MealItemRow";
import { useCreateMeal, useUpdateMeal } from "@/lib/hooks/useMeals";
import { type FoodResult } from "@/lib/types/food";
import { type MealResult, type MealType } from "@/lib/types/meal";
import { ApiError } from "@/lib/types/api";

const MEAL_TYPES: MealType[] = ["Breakfast", "Lunch", "Dinner", "Snack"];

interface DraftItem {
  food: FoodResult;
  grams: number;
}

interface Props {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  date: string;
  // When provided, the dialog is in edit mode and pre-populates the form.
  meal?: MealResult;
}

export function MealLogDialog({ open, onOpenChange, date, meal }: Props) {
  const isEdit = !!meal;

  const [mealType, setMealType] = useState<MealType>("Breakfast");
  const [items, setItems] = useState<DraftItem[]>([]);

  // Pre-populate when editing. The food data on MealItem only has name + id,
  // not the full per-100g macros needed for the live preview. We reconstruct
  // a minimal FoodResult from the snapshotted item macros (per-gram → per-100g).
  useEffect(() => {
    if (!open) return;

    if (meal) {
      setMealType(meal.mealType);
      setItems(
        meal.items.map((item) => ({
          food: {
            id: item.foodId,
            name: item.foodName,
            brand: null,
            isCustom: false,
            // Back-calculate per-100g from the snapshotted item macros.
            calories: item.grams > 0 ? (item.macros.calories / item.grams) * 100 : 0,
            protein:  item.grams > 0 ? (item.macros.protein  / item.grams) * 100 : 0,
            carbs:    item.grams > 0 ? (item.macros.carbs    / item.grams) * 100 : 0,
            fat:      item.grams > 0 ? (item.macros.fat      / item.grams) * 100 : 0,
            fiber:    item.grams > 0 ? (item.macros.fiber    / item.grams) * 100 : 0,
            createdAt: "",
            updatedAt: "",
          },
          grams: item.grams,
        })),
      );
    } else {
      setMealType("Breakfast");
      setItems([]);
    }
  }, [open, meal]);

  const { mutate: create, isPending: isCreating } = useCreateMeal();
  const { mutate: update, isPending: isUpdating } = useUpdateMeal(date);
  const isPending = isCreating || isUpdating;

  function handleFoodSelect(food: FoodResult) {
    // If already in the list, just bump focus rather than adding a duplicate.
    if (items.some((i) => i.food.id === food.id)) return;
    setItems((prev) => [...prev, { food, grams: 100 }]);
  }

  function handleSubmit() {
    if (items.length === 0) {
      toast.error("Add at least one food item.");
      return;
    }

    const payload = {
      mealType,
      date,
      items: items.map((i) => ({ foodId: i.food.id, grams: i.grams })),
    };

    if (isEdit && meal) {
      update(
        { id: meal.id, payload },
        {
          onSuccess: () => { onOpenChange(false); toast.success("Meal updated."); },
          onError: (err) => toast.error(err instanceof ApiError ? err.problem.title : "Failed to update meal."),
        },
      );
    } else {
      create(payload, {
        onSuccess: () => { onOpenChange(false); toast.success("Meal logged."); },
        onError: (err) => toast.error(err instanceof ApiError ? err.problem.title : "Failed to log meal."),
      });
    }
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-lg">
        <DialogHeader>
          <DialogTitle>{isEdit ? "Edit meal" : "Log a meal"}</DialogTitle>
          <DialogDescription>
            {isEdit ? "Update the meal details below." : "Search for foods and set portion sizes."}
          </DialogDescription>
        </DialogHeader>

        <div className="space-y-4 py-2">
          {/* Meal type */}
          <div className="space-y-1.5">
            <Label>Meal type</Label>
            <Select value={mealType} onValueChange={(v) => setMealType(v as MealType)}>
              <SelectTrigger className="w-40">
                <SelectValue />
              </SelectTrigger>
              <SelectContent>
                {MEAL_TYPES.map((t) => (
                  <SelectItem key={t} value={t}>{t}</SelectItem>
                ))}
              </SelectContent>
            </Select>
          </div>

          {/* Food search */}
          <div className="space-y-1.5">
            <Label>Add food</Label>
            <FoodSearchCombobox
              onSelect={handleFoodSelect}
              selectedIds={items.map((i) => i.food.id)}
            />
          </div>

          {/* Items */}
          {items.length > 0 && (
            <div className="space-y-1.5">
              <Label>Items</Label>
              <div className="rounded-md border px-3">
                {items.map((item, i) => (
                  <MealItemRow
                    key={item.food.id}
                    food={item.food}
                    grams={item.grams}
                    onGramsChange={(grams) =>
                      setItems((prev) =>
                        prev.map((x, idx) => (idx === i ? { ...x, grams } : x)),
                      )
                    }
                    onRemove={() =>
                      setItems((prev) => prev.filter((_, idx) => idx !== i))
                    }
                  />
                ))}
              </div>
            </div>
          )}
        </div>

        <DialogFooter>
          <Button variant="ghost" onClick={() => onOpenChange(false)}>
            Cancel
          </Button>
          <Button onClick={handleSubmit} disabled={isPending || items.length === 0}>
            {isPending ? "Saving…" : isEdit ? "Save changes" : "Log meal"}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
