"use client";

import { useState } from "react";
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
import { Label } from "@/components/ui/label";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { createPost } from "@/lib/api/social";
import { socialKeys } from "@/lib/hooks/useSocial";
import { type PostVisibility } from "@/lib/types/social";
import { type MealResult } from "@/lib/types/meal";
import { ApiError } from "@/lib/types/api";

interface Props {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  meal: MealResult;
}

export function CreatePostDialog({ open, onOpenChange, meal }: Props) {
  const [caption, setCaption] = useState("");
  const [visibility, setVisibility] = useState<PostVisibility>("Public");

  const queryClient = useQueryClient();
  const { mutate: share, isPending } = useMutation({
    mutationFn: () => createPost({ mealId: meal.id, caption: caption.trim() || undefined, visibility }),
    onSuccess: () => {
      // Invalidate feed and explore so the new post appears.
      queryClient.invalidateQueries({ queryKey: ["social", "feed"] });
      queryClient.invalidateQueries({ queryKey: ["social", "explore"] });
      toast.success("Shared to feed!");
      setCaption("");
      setVisibility("Public");
      onOpenChange(false);
    },
    onError: (err) => {
      toast.error(err instanceof ApiError ? err.problem.title : "Failed to share meal.");
    },
  });

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-sm">
        <DialogHeader>
          <DialogTitle>Share meal</DialogTitle>
          <DialogDescription>
            Share your {meal.mealType.toLowerCase()} with the community.
          </DialogDescription>
        </DialogHeader>

        <div className="space-y-4 py-2">
          <div className="space-y-1.5">
            <Label htmlFor="caption">Caption <span className="text-muted-foreground">(optional)</span></Label>
            <textarea
              id="caption"
              value={caption}
              onChange={(e) => setCaption(e.target.value)}
              placeholder="What did you think of this meal?"
              maxLength={500}
              rows={3}
              className="w-full rounded-md border bg-background px-3 py-2 text-sm resize-none focus-visible:outline-none focus-visible:ring-1 focus-visible:ring-ring"
            />
          </div>

          <div className="space-y-1.5">
            <Label>Visibility</Label>
            <Select value={visibility} onValueChange={(v) => setVisibility(v as PostVisibility)}>
              <SelectTrigger>
                <SelectValue />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="Public">Public — anyone can see</SelectItem>
                <SelectItem value="FollowersOnly">Followers only</SelectItem>
                <SelectItem value="Private">Private — only me</SelectItem>
              </SelectContent>
            </Select>
          </div>
        </div>

        <DialogFooter>
          <Button variant="ghost" onClick={() => onOpenChange(false)}>Cancel</Button>
          <Button onClick={() => share()} disabled={isPending}>
            {isPending ? "Sharing…" : "Share"}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
