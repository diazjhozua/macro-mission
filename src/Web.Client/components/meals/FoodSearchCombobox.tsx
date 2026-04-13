"use client";

import { useState } from "react";
import { Check, Search } from "lucide-react";
import {
  Command,
  CommandEmpty,
  CommandGroup,
  CommandInput,
  CommandItem,
  CommandList,
} from "@/components/ui/command";
import { useSearchFoods } from "@/lib/hooks/useFoods";
import { type FoodResult } from "@/lib/types/food";
import { cn } from "@/lib/utils";

interface Props {
  onSelect: (food: FoodResult) => void;
  selectedIds: string[];
}

export function FoodSearchCombobox({ onSelect, selectedIds }: Props) {
  const [term, setTerm] = useState("");
  const { data: foods = [], isFetching } = useSearchFoods(term);

  return (
    <Command className="border rounded-md" shouldFilter={false}>
      <CommandInput
        placeholder="Search foods…"
        value={term}
        onValueChange={setTerm}
      />
      <CommandList className="max-h-52">
        {term.length === 0 && (
          <div className="flex items-center gap-2 px-3 py-6 text-sm text-muted-foreground justify-center">
            <Search className="h-4 w-4" />
            Type to search foods
          </div>
        )}

        {term.length > 0 && !isFetching && foods.length === 0 && (
          <CommandEmpty>No foods found for &ldquo;{term}&rdquo;</CommandEmpty>
        )}

        {foods.length > 0 && (
          <CommandGroup>
            {foods.map((food) => {
              const already = selectedIds.includes(food.id);
              return (
                <CommandItem
                  key={food.id}
                  value={food.id}
                  onSelect={() => onSelect(food)}
                  className="flex items-center justify-between gap-3"
                >
                  <div className="min-w-0">
                    <p className="font-medium truncate">{food.name}</p>
                    {food.brand && (
                      <p className="text-xs text-muted-foreground truncate">{food.brand}</p>
                    )}
                  </div>
                  <div className="flex items-center gap-3 shrink-0 text-xs text-muted-foreground tabular-nums">
                    <span>{Math.round(food.calories)} kcal</span>
                    <span className="hidden sm:inline">per 100g</span>
                    {already && <Check className="h-3.5 w-3.5 text-primary" />}
                  </div>
                </CommandItem>
              );
            })}
          </CommandGroup>
        )}
      </CommandList>
    </Command>
  );
}
