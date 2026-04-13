"use client";

import { useState } from "react";
import { ChevronLeft, ChevronRight, Pencil, Plus, Trash2 } from "lucide-react";
import { toast } from "sonner";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { Card, CardContent, CardHeader } from "@/components/ui/card";
import { MealLogDialog } from "@/components/meals/MealLogDialog";
import { useMealsByDate, useDeleteMeal } from "@/lib/hooks/useMeals";
import { type MealResult } from "@/lib/types/meal";
import { addDays, formatDisplayDate, isSameDay, toApiDate } from "@/lib/utils/date";

const MEAL_TYPE_COLORS: Record<string, string> = {
  Breakfast: "bg-amber-50 text-amber-700 border-amber-200",
  Lunch:     "bg-green-50 text-green-700 border-green-200",
  Dinner:    "bg-blue-50 text-blue-700 border-blue-200",
  Snack:     "bg-purple-50 text-purple-700 border-purple-200",
};

export default function MealsPage() {
  const [date, setDate] = useState(() => new Date());
  const [dialogOpen, setDialogOpen] = useState(false);
  const [editingMeal, setEditingMeal] = useState<MealResult | undefined>();

  const dateStr = toApiDate(date);
  const isToday = isSameDay(date, new Date());

  const { data: meals = [], isLoading } = useMealsByDate(dateStr);
  const { mutate: deleteMeal } = useDeleteMeal(dateStr);

  function handleEdit(meal: MealResult) {
    setEditingMeal(meal);
    setDialogOpen(true);
  }

  function handleAdd() {
    setEditingMeal(undefined);
    setDialogOpen(true);
  }

  function handleDelete(meal: MealResult) {
    deleteMeal(meal.id, {
      onSuccess: () => toast.success("Meal deleted."),
      onError:   () => toast.error("Failed to delete meal."),
    });
  }

  return (
    <div className="max-w-2xl mx-auto space-y-6">

      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-xl font-semibold">{isToday ? "Today" : formatDisplayDate(date)}</h1>
          {!isToday && (
            <p className="text-sm text-muted-foreground">{formatDisplayDate(date)}</p>
          )}
        </div>
        <div className="flex items-center gap-2">
          <div className="flex items-center gap-1">
            <Button variant="ghost" size="icon" onClick={() => setDate((d) => addDays(d, -1))}>
              <ChevronLeft className="h-4 w-4" />
            </Button>
            {!isToday && (
              <Button variant="ghost" size="sm" className="text-xs" onClick={() => setDate(new Date())}>
                Today
              </Button>
            )}
            <Button variant="ghost" size="icon" disabled={isToday} onClick={() => setDate((d) => addDays(d, 1))}>
              <ChevronRight className="h-4 w-4" />
            </Button>
          </div>
          <Button size="sm" onClick={handleAdd} className="gap-1.5">
            <Plus className="h-4 w-4" />
            Log meal
          </Button>
        </div>
      </div>

      {/* Meals list */}
      {isLoading && (
        <div className="space-y-3">
          {[1, 2].map((i) => (
            <div key={i} className="h-28 rounded-xl bg-muted animate-pulse" />
          ))}
        </div>
      )}

      {!isLoading && meals.length === 0 && (
        <div className="text-center py-16 text-muted-foreground">
          <p className="text-sm">No meals logged {isToday ? "today" : "on this day"}.</p>
          <Button variant="link" size="sm" className="mt-1" onClick={handleAdd}>
            Log your first meal
          </Button>
        </div>
      )}

      <div className="space-y-3">
        {meals.map((meal) => (
          <Card key={meal.id}>
            <CardHeader className="pb-3 pt-4 px-5">
              <div className="flex items-center justify-between">
                <Badge variant="outline" className={MEAL_TYPE_COLORS[meal.mealType]}>
                  {meal.mealType}
                </Badge>
                <div className="flex items-center gap-1">
                  <span className="text-sm font-semibold tabular-nums mr-2">
                    {Math.round(meal.totals.calories)} kcal
                  </span>
                  <Button
                    variant="ghost"
                    size="icon"
                    className="h-7 w-7 text-muted-foreground"
                    onClick={() => handleEdit(meal)}
                  >
                    <Pencil className="h-3.5 w-3.5" />
                  </Button>
                  <Button
                    variant="ghost"
                    size="icon"
                    className="h-7 w-7 text-muted-foreground hover:text-destructive"
                    onClick={() => handleDelete(meal)}
                  >
                    <Trash2 className="h-3.5 w-3.5" />
                  </Button>
                </div>
              </div>
            </CardHeader>

            <CardContent className="px-5 pb-4 space-y-1.5">
              {meal.items.map((item, i) => (
                <div key={i} className="flex items-center justify-between text-sm">
                  <span>{item.foodName}</span>
                  <div className="flex gap-3 text-muted-foreground tabular-nums shrink-0 ml-4">
                    <span>{item.grams}g</span>
                    <span className="w-16 text-right">{Math.round(item.macros.calories)} kcal</span>
                  </div>
                </div>
              ))}
              <div className="flex gap-4 pt-2 border-t text-xs text-muted-foreground tabular-nums">
                <span>P {Math.round(meal.totals.protein)}g</span>
                <span>C {Math.round(meal.totals.carbs)}g</span>
                <span>F {Math.round(meal.totals.fat)}g</span>
                <span>Fiber {Math.round(meal.totals.fiber)}g</span>
              </div>
            </CardContent>
          </Card>
        ))}
      </div>

      <MealLogDialog
        open={dialogOpen}
        onOpenChange={(open) => {
          setDialogOpen(open);
          if (!open) setEditingMeal(undefined);
        }}
        date={dateStr}
        meal={editingMeal}
      />
    </div>
  );
}
