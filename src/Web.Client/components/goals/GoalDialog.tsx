"use client";

import { useEffect } from "react";
import { useForm, type Resolver } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
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
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { useCreateDailyGoal, useUpdateDailyGoal } from "@/lib/hooks/useDailyGoals";
import { type DailyGoalResult } from "@/lib/types/dailyGoal";
import { ApiError } from "@/lib/types/api";

const schema = z.object({
  name:     z.string().min(1, "Name is required"),
  calories: z.coerce.number().min(0, "Must be 0 or more"),
  protein:  z.coerce.number().min(0, "Must be 0 or more"),
  carbs:    z.coerce.number().min(0, "Must be 0 or more"),
  fat:      z.coerce.number().min(0, "Must be 0 or more"),
  fiber:    z.coerce.number().min(0, "Must be 0 or more"),
});

type FormValues = z.infer<typeof schema>;

interface Props {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  goal?: DailyGoalResult;
}

export function GoalDialog({ open, onOpenChange, goal }: Props) {
  const isEdit = !!goal;

  const { mutate: create, isPending: isCreating } = useCreateDailyGoal();
  const { mutate: update, isPending: isUpdating } = useUpdateDailyGoal();
  const isPending = isCreating || isUpdating;

  const {
    register,
    handleSubmit,
    reset,
    setError,
    formState: { errors },
  } = useForm<FormValues>({ resolver: zodResolver(schema) as Resolver<FormValues> });

  useEffect(() => {
    if (!open) return;
    reset(
      goal
        ? { name: goal.name, calories: goal.calories, protein: goal.protein, carbs: goal.carbs, fat: goal.fat, fiber: goal.fiber }
        : { name: "", calories: 2000, protein: 150, carbs: 200, fat: 65, fiber: 25 },
    );
  }, [open, goal, reset]);

  function onSubmit(values: FormValues) {
    if (isEdit && goal) {
      // Preserve isActive — we're editing macros/name, not toggling active state.
      update(
        { id: goal.id, payload: { ...values, isActive: goal.isActive } },
        {
          onSuccess: () => { onOpenChange(false); toast.success("Goal updated."); },
          onError: handleError,
        },
      );
    } else {
      create(values, {
        onSuccess: () => { onOpenChange(false); toast.success("Goal created."); },
        onError: handleError,
      });
    }
  }

  function handleError(err: unknown) {
    if (err instanceof ApiError && err.isValidation()) {
      Object.entries(err.problem.errors).forEach(([field, messages]) => {
        setError(field as keyof FormValues, { message: messages[0] });
      });
    } else {
      toast.error("Something went wrong. Please try again.");
    }
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-md">
        <DialogHeader>
          <DialogTitle>{isEdit ? "Edit goal" : "New daily goal"}</DialogTitle>
          <DialogDescription>Set your daily nutrition targets.</DialogDescription>
        </DialogHeader>

        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4 py-2">
          <div className="space-y-1.5">
            <Label htmlFor="name">Goal name</Label>
            <Input id="name" placeholder="e.g. Cutting, Maintenance" {...register("name")} />
            {errors.name && <p className="text-sm text-destructive">{errors.name.message}</p>}
          </div>

          <div className="grid grid-cols-2 gap-3">
            {(["calories", "protein", "carbs", "fat", "fiber"] as const).map((macro) => (
              <div key={macro} className="space-y-1.5">
                <Label htmlFor={macro} className="capitalize">{macro}</Label>
                <div className="relative">
                  <Input
                    id={macro}
                    type="number"
                    step="1"
                    min="0"
                    className="pr-12"
                    {...register(macro)}
                  />
                  <span className="absolute right-3 top-1/2 -translate-y-1/2 text-xs text-muted-foreground pointer-events-none">
                    {macro === "calories" ? "kcal" : "g"}
                  </span>
                </div>
                {errors[macro] && (
                  <p className="text-sm text-destructive">{errors[macro]?.message}</p>
                )}
              </div>
            ))}
          </div>

          <DialogFooter>
            <Button type="button" variant="ghost" onClick={() => onOpenChange(false)}>Cancel</Button>
            <Button type="submit" disabled={isPending}>
              {isPending ? "Saving…" : isEdit ? "Save changes" : "Create goal"}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}
