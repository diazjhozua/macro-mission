"use client";

import { useState } from "react";
import { Button } from "@/components/ui/button";
import { PostCard } from "@/components/social/PostCard";
import { useUserPosts } from "@/lib/hooks/useSocial";

interface Props {
  userId: string;
}

export function UserPostsGrid({ userId }: Props) {
  const [pages, setPages] = useState([1]);
  const queries = pages.map((page) => useUserPosts(userId, page));

  const allPosts = queries.flatMap((q) => q.data ?? []);
  const lastPage  = queries[queries.length - 1];
  const hasMore   = (lastPage.data?.length ?? 0) >= 20;
  const isLoading = queries[0].isLoading;

  if (isLoading) {
    return (
      <div className="space-y-3">
        {[1, 2].map((i) => <div key={i} className="h-28 bg-muted rounded-xl animate-pulse" />)}
      </div>
    );
  }

  if (allPosts.length === 0) {
    return <p className="text-sm text-muted-foreground text-center py-10">No posts yet.</p>;
  }

  return (
    <div className="space-y-3">
      {allPosts.map((post) => <PostCard key={post.id} post={post} />)}

      {hasMore && (
        <Button
          variant="outline"
          className="w-full"
          onClick={() => setPages((p) => [...p, p.length + 1])}
          disabled={lastPage.isFetching}
        >
          {lastPage.isFetching ? "Loading…" : "Load more"}
        </Button>
      )}
    </div>
  );
}
