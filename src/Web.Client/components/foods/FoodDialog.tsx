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
import { useCreateFood, useUpdateFood } from "@/lib/hooks/useFoods";
import { type FoodResult } from "@/lib/types/food";
import { ApiError } from "@/lib/types/api";

const schema = z.object({
  name:     z.string().min(1, "Name is required"),
  brand:    z.string().optional(),
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
  // When provided the dialog is in edit mode.
  food?: FoodResult;
}

export function FoodDialog({ open, onOpenChange, food }: Props) {
  const isEdit = !!food;

  const { mutate: create, isPending: isCreating } = useCreateFood();
  const { mutate: update, isPending: isUpdating } = useUpdateFood();
  const isPending = isCreating || isUpdating;

  const {
    register,
    handleSubmit,
    reset,
    setError,
    formState: { errors },
  } = useForm<FormValues>({ resolver: zodResolver(schema) as Resolver<FormValues> });

  // Pre-populate when editing; clear when opening for create.
  useEffect(() => {
    if (!open) return;
    reset(
      food
        ? { name: food.name, brand: food.brand ?? "", calories: food.calories, protein: food.protein, carbs: food.carbs, fat: food.fat, fiber: food.fiber }
        : { name: "", brand: "", calories: 0, protein: 0, carbs: 0, fat: 0, fiber: 0 },
    );
  }, [open, food, reset]);

  function onSubmit(values: FormValues) {
    const payload = { ...values, brand: values.brand || undefined };

    if (isEdit && food) {
      update(
        { id: food.id, payload },
        {
          onSuccess: () => { onOpenChange(false); toast.success("Food updated."); },
          onError: handleError,
        },
      );
    } else {
      create(payload, {
        onSuccess: () => { onOpenChange(false); toast.success("Food created."); },
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
          <DialogTitle>{isEdit ? "Edit food" : "Add custom food"}</DialogTitle>
          <DialogDescription>All macros are per 100g.</DialogDescription>
        </DialogHeader>

        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4 py-2">
          <div className="space-y-1.5">
            <Label htmlFor="name">Name</Label>
            <Input id="name" {...register("name")} />
            {errors.name && <p className="text-sm text-destructive">{errors.name.message}</p>}
          </div>

          <div className="space-y-1.5">
            <Label htmlFor="brand">Brand <span className="text-muted-foreground">(optional)</span></Label>
            <Input id="brand" {...register("brand")} />
          </div>

          {/* Macro grid */}
          <div className="grid grid-cols-2 gap-3">
            {(["calories", "protein", "carbs", "fat", "fiber"] as const).map((macro) => (
              <div key={macro} className="space-y-1.5">
                <Label htmlFor={macro} className="capitalize">{macro}</Label>
                <div className="relative">
                  <Input
                    id={macro}
                    type="number"
                    step="0.1"
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
              {isPending ? "Saving…" : isEdit ? "Save changes" : "Add food"}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}
