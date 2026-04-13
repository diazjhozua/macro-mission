import { Pencil, Trash2 } from "lucide-react";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader } from "@/components/ui/card";
import { type DailyGoalResult } from "@/lib/types/dailyGoal";

interface Props {
  goal: DailyGoalResult;
  onEdit: () => void;
  onDelete: () => void;
  onSetActive: () => void;
  isSettingActive: boolean;
}

export function GoalCard({ goal, onEdit, onDelete, onSetActive, isSettingActive }: Props) {
  return (
    <Card className={goal.isActive ? "border-primary/40 bg-primary/5" : undefined}>
      <CardHeader className="pb-3 pt-4 px-5">
        <div className="flex items-center justify-between">
          <div className="flex items-center gap-2">
            <span className="font-semibold">{goal.name}</span>
            {goal.isActive && (
              <Badge className="text-xs bg-primary/10 text-primary border-primary/20" variant="outline">
                Active
              </Badge>
            )}
          </div>
          <div className="flex items-center gap-1">
            <Button variant="ghost" size="icon" className="h-7 w-7 text-muted-foreground" onClick={onEdit}>
              <Pencil className="h-3.5 w-3.5" />
            </Button>
            <Button
              variant="ghost"
              size="icon"
              className="h-7 w-7 text-muted-foreground hover:text-destructive"
              onClick={onDelete}
              // Prevent deleting the active goal — set another as active first.
              disabled={goal.isActive}
            >
              <Trash2 className="h-3.5 w-3.5" />
            </Button>
          </div>
        </div>
      </CardHeader>

      <CardContent className="px-5 pb-4 space-y-3">
        <div className="grid grid-cols-5 gap-2 text-center">
          {(["calories", "protein", "carbs", "fat", "fiber"] as const).map((macro) => (
            <div key={macro}>
              <p className="text-sm font-semibold tabular-nums">{Math.round(goal[macro])}</p>
              <p className="text-xs text-muted-foreground capitalize">
                {macro === "calories" ? "kcal" : macro}
              </p>
            </div>
          ))}
        </div>

        {!goal.isActive && (
          <Button
            variant="outline"
            size="sm"
            className="w-full text-xs"
            onClick={onSetActive}
            disabled={isSettingActive}
          >
            {isSettingActive ? "Setting…" : "Set as active"}
          </Button>
        )}
      </CardContent>
    </Card>
  );
}
