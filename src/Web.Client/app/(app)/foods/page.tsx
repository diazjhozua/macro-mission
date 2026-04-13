"use client";

import { useState } from "react";
import { Pencil, Plus, Trash2 } from "lucide-react";
import { toast } from "sonner";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Badge } from "@/components/ui/badge";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { FoodDialog } from "@/components/foods/FoodDialog";
import { useDeleteFood, useSearchFoods } from "@/lib/hooks/useFoods";
import { type FoodResult } from "@/lib/types/food";

export default function FoodsPage() {
  const [term, setTerm] = useState("");
  const [dialogOpen, setDialogOpen] = useState(false);
  const [editingFood, setEditingFood] = useState<FoodResult | undefined>();

  // alwaysFire=true so the table shows all foods on mount before any search.
  const { data: foods = [], isFetching } = useSearchFoods(term, true);
  const { mutate: deleteFood } = useDeleteFood();

  function handleEdit(food: FoodResult) {
    setEditingFood(food);
    setDialogOpen(true);
  }

  function handleAdd() {
    setEditingFood(undefined);
    setDialogOpen(true);
  }

  function handleDelete(food: FoodResult) {
    deleteFood(food.id, {
      onSuccess: () => toast.success(`"${food.name}" deleted.`),
      onError:   () => toast.error("Failed to delete food."),
    });
  }

  return (
    <div className="max-w-4xl mx-auto space-y-4">
      <div className="flex items-center justify-between">
        <h1 className="text-xl font-semibold">Foods</h1>
        <Button size="sm" onClick={handleAdd} className="gap-1.5">
          <Plus className="h-4 w-4" />
          Add food
        </Button>
      </div>

      <Input
        placeholder="Search foods…"
        value={term}
        onChange={(e) => setTerm(e.target.value)}
        className="max-w-sm"
      />

      <div className="rounded-md border">
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead>Name</TableHead>
              <TableHead className="hidden sm:table-cell">Brand</TableHead>
              <TableHead className="text-right">Cal</TableHead>
              <TableHead className="text-right hidden md:table-cell">Protein</TableHead>
              <TableHead className="text-right hidden md:table-cell">Carbs</TableHead>
              <TableHead className="text-right hidden md:table-cell">Fat</TableHead>
              <TableHead className="text-right hidden md:table-cell">Fiber</TableHead>
              <TableHead className="w-20"></TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {isFetching && foods.length === 0 && (
              <TableRow>
                <TableCell colSpan={8} className="text-center text-muted-foreground py-10 text-sm">
                  Loading…
                </TableCell>
              </TableRow>
            )}

            {!isFetching && foods.length === 0 && (
              <TableRow>
                <TableCell colSpan={8} className="text-center text-muted-foreground py-10 text-sm">
                  {term ? `No foods found for "${term}".` : "No foods yet."}
                </TableCell>
              </TableRow>
            )}

            {foods.map((food) => (
              <TableRow key={food.id}>
                <TableCell>
                  <div className="flex items-center gap-2">
                    <span className="font-medium">{food.name}</span>
                    {food.isCustom && (
                      <Badge variant="outline" className="text-xs text-blue-600 border-blue-200 bg-blue-50">
                        Custom
                      </Badge>
                    )}
                  </div>
                </TableCell>
                <TableCell className="hidden sm:table-cell text-muted-foreground text-sm">
                  {food.brand ?? "—"}
                </TableCell>
                <TableCell className="text-right tabular-nums text-sm">{Math.round(food.calories)}</TableCell>
                <TableCell className="text-right tabular-nums text-sm hidden md:table-cell">{Math.round(food.protein)}g</TableCell>
                <TableCell className="text-right tabular-nums text-sm hidden md:table-cell">{Math.round(food.carbs)}g</TableCell>
                <TableCell className="text-right tabular-nums text-sm hidden md:table-cell">{Math.round(food.fat)}g</TableCell>
                <TableCell className="text-right tabular-nums text-sm hidden md:table-cell">{Math.round(food.fiber)}g</TableCell>
                <TableCell>
                  {/* Edit and delete are only available for custom foods —
                      global foods are read-only. */}
                  {food.isCustom && (
                    <div className="flex items-center justify-end gap-1">
                      <Button
                        variant="ghost"
                        size="icon"
                        className="h-7 w-7 text-muted-foreground"
                        onClick={() => handleEdit(food)}
                      >
                        <Pencil className="h-3.5 w-3.5" />
                      </Button>
                      <Button
                        variant="ghost"
                        size="icon"
                        className="h-7 w-7 text-muted-foreground hover:text-destructive"
                        onClick={() => handleDelete(food)}
                      >
                        <Trash2 className="h-3.5 w-3.5" />
                      </Button>
                    </div>
                  )}
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </div>

      <FoodDialog
        open={dialogOpen}
        onOpenChange={(open) => {
          setDialogOpen(open);
          if (!open) setEditingFood(undefined);
        }}
        food={editingFood}
      />
    </div>
  );
}
