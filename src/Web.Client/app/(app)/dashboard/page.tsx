"use client";

import { useState } from "react";
import { ChevronLeft, ChevronRight } from "lucide-react";
import { Button } from "@/components/ui/button";
import { MacroProgressCard } from "@/components/dashboard/MacroProgressCard";
import { MealCard } from "@/components/dashboard/MealCard";
import { useDailySummary } from "@/lib/hooks/useMeals";
import { addDays, formatDisplayDate, isSameDay, toApiDate } from "@/lib/utils/date";
import { MACRO_KEYS } from "@/lib/utils/macros";

export default function DashboardPage() {
  const [date, setDate] = useState(() => new Date());
  const dateStr = toApiDate(date);
  const isToday = isSameDay(date, new Date());

  const { data: summary, isLoading } = useDailySummary(dateStr);

  return (
    <div className="max-w-2xl mx-auto space-y-6">

      {/* Date navigation */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-xl font-semibold">{isToday ? "Today" : formatDisplayDate(date)}</h1>
          {!isToday && (
            <p className="text-sm text-muted-foreground">{formatDisplayDate(date)}</p>
          )}
        </div>
        <div className="flex items-center gap-1">
          <Button
            variant="ghost"
            size="icon"
            onClick={() => setDate((d) => addDays(d, -1))}
          >
            <ChevronLeft className="h-4 w-4" />
          </Button>
          {!isToday && (
            <Button
              variant="ghost"
              size="sm"
              className="text-xs"
              onClick={() => setDate(new Date())}
            >
              Today
            </Button>
          )}
          <Button
            variant="ghost"
            size="icon"
            disabled={isToday}
            onClick={() => setDate((d) => addDays(d, 1))}
          >
            <ChevronRight className="h-4 w-4" />
          </Button>
        </div>
      </div>

      {/* Macro progress cards */}
      {isLoading ? (
        <div className="grid grid-cols-2 sm:grid-cols-3 gap-3">
          {MACRO_KEYS.map((k) => (
            <div key={k} className="h-24 rounded-xl bg-muted animate-pulse" />
          ))}
        </div>
      ) : (
        <div className="grid grid-cols-2 sm:grid-cols-3 gap-3">
          {MACRO_KEYS.map((macro) => (
            <MacroProgressCard
              key={macro}
              macro={macro}
              consumed={summary?.consumed[macro] ?? 0}
              goal={summary?.goal?.[macro] ?? null}
            />
          ))}
        </div>
      )}

      {/* Meals list */}
      <div className="space-y-3">
        <h2 className="text-sm font-medium text-muted-foreground uppercase tracking-wide">
          Meals
        </h2>

        {!isLoading && (!summary?.meals || summary.meals.length === 0) && (
          <p className="text-sm text-muted-foreground py-6 text-center">
            No meals logged {isToday ? "today" : "on this day"}.
          </p>
        )}

        {summary?.meals.map((meal) => (
          <MealCard key={meal.id} meal={meal} />
        ))}
      </div>
    </div>
  );
}
