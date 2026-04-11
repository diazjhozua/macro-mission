import { Card, CardContent } from "@/components/ui/card";
import { Progress, ProgressIndicator, ProgressTrack } from "@/components/ui/progress";
import { calcPercent, MACRO_COLORS, MACRO_LABELS, MACRO_UNITS } from "@/lib/utils/macros";
import { cn } from "@/lib/utils";

interface Props {
  macro: "calories" | "protein" | "carbs" | "fat" | "fiber";
  consumed: number;
  // When null there's no active goal — show consumed only, no progress bar.
  goal: number | null;
}

export function MacroProgressCard({ macro, consumed, goal }: Props) {
  const percent = goal !== null ? calcPercent(consumed, goal) : null;
  const isOver  = goal !== null && consumed > goal;

  return (
    <Card>
      <CardContent className="pt-5 pb-4 px-5">
        <div className="flex items-baseline justify-between mb-3">
          <span className="text-sm font-medium text-muted-foreground">
            {MACRO_LABELS[macro]}
          </span>
          <div className="text-right">
            <span className={cn("text-lg font-semibold tabular-nums", isOver && "text-destructive")}>
              {Math.round(consumed)}
            </span>
            {goal !== null ? (
              <span className="text-sm text-muted-foreground ml-1">
                / {Math.round(goal)} {MACRO_UNITS[macro]}
              </span>
            ) : (
              <span className="text-sm text-muted-foreground ml-1">
                {MACRO_UNITS[macro]}
              </span>
            )}
          </div>
        </div>

        {percent !== null && (
          <div className="space-y-1">
            <Progress value={percent} className="block">
              <ProgressTrack>
                <ProgressIndicator className={MACRO_COLORS[macro]} />
              </ProgressTrack>
            </Progress>
            <p className="text-xs text-muted-foreground text-right">{percent}%</p>
          </div>
        )}
      </CardContent>
    </Card>
  );
}
