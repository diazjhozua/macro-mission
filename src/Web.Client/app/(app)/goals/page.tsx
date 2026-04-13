"use client";

import { useState } from "react";
import { Plus } from "lucide-react";
import { toast } from "sonner";
import { Button } from "@/components/ui/button";
import { GoalCard } from "@/components/goals/GoalCard";
import { GoalDialog } from "@/components/goals/GoalDialog";
import { useDailyGoals, useDeleteDailyGoal, useUpdateDailyGoal } from "@/lib/hooks/useDailyGoals";
import { type DailyGoalResult } from "@/lib/types/dailyGoal";

export default function GoalsPage() {
  const [dialogOpen, setDialogOpen] = useState(false);
  const [editingGoal, setEditingGoal] = useState<DailyGoalResult | undefined>();
  const [settingActiveId, setSettingActiveId] = useState<string | null>(null);

  const { data: goals = [], isLoading } = useDailyGoals();
  const { mutate: updateGoal } = useUpdateDailyGoal();
  const { mutate: deleteGoal } = useDeleteDailyGoal();

  function handleEdit(goal: DailyGoalResult) {
    setEditingGoal(goal);
    setDialogOpen(true);
  }

  function handleAdd() {
    setEditingGoal(undefined);
    setDialogOpen(true);
  }

  function handleSetActive(goal: DailyGoalResult) {
    setSettingActiveId(goal.id);
    updateGoal(
      { id: goal.id, payload: { name: goal.name, isActive: true, calories: goal.calories, protein: goal.protein, carbs: goal.carbs, fat: goal.fat, fiber: goal.fiber } },
      {
        onSuccess: () => toast.success(`"${goal.name}" is now your active goal.`),
        onError:   () => toast.error("Failed to set active goal."),
        onSettled: () => setSettingActiveId(null),
      },
    );
  }

  function handleDelete(goal: DailyGoalResult) {
    deleteGoal(goal.id, {
      onSuccess: () => toast.success(`"${goal.name}" deleted.`),
      onError:   () => toast.error("Failed to delete goal."),
    });
  }

  // Active goal first, then alphabetical.
  const sorted = [...goals].sort((a, b) => {
    if (a.isActive !== b.isActive) return a.isActive ? -1 : 1;
    return a.name.localeCompare(b.name);
  });

  return (
    <div className="max-w-2xl mx-auto space-y-4">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-xl font-semibold">Daily Goals</h1>
          <p className="text-sm text-muted-foreground mt-0.5">
            The active goal is used for progress tracking on the dashboard.
          </p>
        </div>
        <Button size="sm" onClick={handleAdd} className="gap-1.5">
          <Plus className="h-4 w-4" />
          New goal
        </Button>
      </div>

      {isLoading && (
        <div className="space-y-3">
          {[1, 2].map((i) => (
            <div key={i} className="h-36 rounded-xl bg-muted animate-pulse" />
          ))}
        </div>
      )}

      {!isLoading && goals.length === 0 && (
        <div className="text-center py-16 text-muted-foreground">
          <p className="text-sm">No goals yet.</p>
          <Button variant="link" size="sm" className="mt-1" onClick={handleAdd}>
            Create your first goal
          </Button>
        </div>
      )}

      <div className="space-y-3">
        {sorted.map((goal) => (
          <GoalCard
            key={goal.id}
            goal={goal}
            onEdit={() => handleEdit(goal)}
            onDelete={() => handleDelete(goal)}
            onSetActive={() => handleSetActive(goal)}
            isSettingActive={settingActiveId === goal.id}
          />
        ))}
      </div>

      <GoalDialog
        open={dialogOpen}
        onOpenChange={(open) => {
          setDialogOpen(open);
          if (!open) setEditingGoal(undefined);
        }}
        goal={editingGoal}
      />
    </div>
  );
}
