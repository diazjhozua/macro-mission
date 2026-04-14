"use client";

import { useState } from "react";
import { Heart } from "lucide-react";
import { Button } from "@/components/ui/button";
import { useLikePost, useUnlikePost } from "@/lib/hooks/useSocial";
import { cn } from "@/lib/utils";

interface Props {
  postId: string;
  likesCount: number;
}

// The backend doesn't return whether the current user has liked the post, so
// we track it in local state. The state resets on page reload, which is an
// acceptable tradeoff given the API surface available.
export function LikeButton({ postId, likesCount }: Props) {
  const [liked, setLiked] = useState(false);

  const { mutate: like,   isPending: isLiking }   = useLikePost(postId);
  const { mutate: unlike, isPending: isUnliking } = useUnlikePost(postId);
  const isPending = isLiking || isUnliking;

  function toggle() {
    if (liked) {
      unlike(undefined, { onSuccess: () => setLiked(false) });
    } else {
      like(undefined, {
        onSuccess: () => setLiked(true),
        // 409 means already liked — flip to liked state so the next click unlikes.
        onError: (err: unknown) => {
          const status = (err as { status?: number }).status;
          if (status === 409) setLiked(true);
        },
      });
    }
  }

  return (
    <Button
      variant="ghost"
      size="sm"
      className={cn("gap-1.5 text-muted-foreground", liked && "text-rose-500 hover:text-rose-500")}
      onClick={toggle}
      disabled={isPending}
    >
      <Heart className={cn("h-4 w-4", liked && "fill-current")} />
      <span className="tabular-nums">{likesCount}</span>
    </Button>
  );
}
