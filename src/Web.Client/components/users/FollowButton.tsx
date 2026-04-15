"use client";

import { useState } from "react";
import { Button } from "@/components/ui/button";
import { useFollowUser, useUnfollowUser } from "@/lib/hooks/useSocial";

interface Props {
  userId: string;
  // Pass true if the current user is already following — checked at the
  // page level by scanning the followers list for the current user's ID.
  initiallyFollowing: boolean;
}

export function FollowButton({ userId, initiallyFollowing }: Props) {
  const [following, setFollowing] = useState(initiallyFollowing);

  const { mutate: follow,   isPending: isFollowing }   = useFollowUser(userId);
  const { mutate: unfollow, isPending: isUnfollowing } = useUnfollowUser(userId);
  const isPending = isFollowing || isUnfollowing;

  function toggle() {
    if (following) {
      unfollow(undefined, {
        onSuccess: () => setFollowing(false),
      });
    } else {
      follow(undefined, {
        onSuccess: () => setFollowing(true),
        // 409 = already following — sync local state.
        onError: (err: unknown) => {
          if ((err as { status?: number }).status === 409) setFollowing(true);
        },
      });
    }
  }

  return (
    <Button
      variant={following ? "outline" : "default"}
      size="sm"
      onClick={toggle}
      disabled={isPending}
    >
      {isPending ? "…" : following ? "Following" : "Follow"}
    </Button>
  );
}
