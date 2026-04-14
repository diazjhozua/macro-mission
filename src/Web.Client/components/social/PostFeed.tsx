"use client";

import { useState } from "react";
import { Button } from "@/components/ui/button";
import { PostCard } from "@/components/social/PostCard";
import { type PostResult } from "@/lib/types/social";

interface Page {
  data: PostResult[] | undefined;
  isLoading: boolean;
}

interface Props {
  // Each loaded page is passed in as an array so the parent controls fetching.
  pages: Page[];
  hasMore: boolean;
  onLoadMore: () => void;
  isLoadingMore: boolean;
  emptyMessage?: string;
}

export function PostFeed({ pages, hasMore, onLoadMore, isLoadingMore, emptyMessage = "Nothing here yet." }: Props) {
  const allPosts = pages.flatMap((p) => p.data ?? []);
  const isInitialLoading = pages.length === 1 && pages[0].isLoading;

  if (isInitialLoading) {
    return (
      <div className="space-y-3">
        {[1, 2, 3].map((i) => (
          <div key={i} className="h-28 rounded-xl bg-muted animate-pulse" />
        ))}
      </div>
    );
  }

  if (allPosts.length === 0) {
    return (
      <p className="text-center text-sm text-muted-foreground py-16">{emptyMessage}</p>
    );
  }

  return (
    <div className="space-y-3">
      {allPosts.map((post) => (
        <PostCard key={post.id} post={post} />
      ))}

      {hasMore && (
        <Button
          variant="outline"
          className="w-full"
          onClick={onLoadMore}
          disabled={isLoadingMore}
        >
          {isLoadingMore ? "Loading…" : "Load more"}
        </Button>
      )}
    </div>
  );
}
