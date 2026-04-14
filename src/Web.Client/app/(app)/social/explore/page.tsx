"use client";

import { useState } from "react";
import { PostFeed } from "@/components/social/PostFeed";
import { useExplore } from "@/lib/hooks/useSocial";

export default function ExplorePage() {
  const [pages, setPages] = useState([1]);
  const queries = pages.map((page) => useExplore(page));

  const lastPage = queries[queries.length - 1];
  const hasMore = (lastPage.data?.length ?? 0) >= 20;

  function loadMore() {
    setPages((prev) => [...prev, prev.length + 1]);
  }

  return (
    <div className="max-w-xl mx-auto space-y-4">
      <h1 className="text-xl font-semibold">Explore</h1>
      <PostFeed
        pages={queries.map((q) => ({ data: q.data, isLoading: q.isLoading }))}
        hasMore={hasMore}
        onLoadMore={loadMore}
        isLoadingMore={lastPage.isFetching && pages.length > 1}
        emptyMessage="No public posts yet."
      />
    </div>
  );
}
