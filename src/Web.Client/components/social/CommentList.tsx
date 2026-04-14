"use client";

import { useState } from "react";
import { Trash2 } from "lucide-react";
import { toast } from "sonner";
import { Button } from "@/components/ui/button";
import { useComments, useDeleteComment } from "@/lib/hooks/useSocial";
import { useCurrentUser } from "@/lib/hooks/useCurrentUser";

interface Props {
  postId: string;
}

export function CommentList({ postId }: Props) {
  const [pages, setPages] = useState([1]);
  const { data: currentUser } = useCurrentUser();
  const { mutate: deleteComment } = useDeleteComment(postId);

  const queries = pages.map((page) => useComments(postId, page));
  const allComments = queries.flatMap((q) => q.data ?? []);
  const lastPage = queries[queries.length - 1];
  const hasMore = (lastPage.data?.length ?? 0) >= 20;
  const isLoading = queries[0].isLoading;

  function handleDelete(commentId: string) {
    deleteComment(commentId, {
      onError: () => toast.error("Failed to delete comment."),
    });
  }

  if (isLoading) {
    return <div className="space-y-2">{[1, 2].map((i) => <div key={i} className="h-10 bg-muted rounded animate-pulse" />)}</div>;
  }

  if (allComments.length === 0) {
    return <p className="text-sm text-muted-foreground">No comments yet. Be the first.</p>;
  }

  return (
    <div className="space-y-3">
      {allComments.map((comment) => (
        <div key={comment.id} className="flex items-start justify-between gap-3">
          <div className="space-y-0.5 min-w-0">
            <span className="text-xs font-medium text-muted-foreground">
              @{comment.authorId.slice(-6)}
            </span>
            <p className="text-sm break-words">{comment.text}</p>
          </div>

          {/* Only show delete for the current user's own comments. */}
          {currentUser?.id === comment.authorId && (
            <Button
              variant="ghost"
              size="icon"
              className="h-6 w-6 shrink-0 text-muted-foreground hover:text-destructive"
              onClick={() => handleDelete(comment.id)}
            >
              <Trash2 className="h-3 w-3" />
            </Button>
          )}
        </div>
      ))}

      {hasMore && (
        <Button
          variant="ghost"
          size="sm"
          className="text-xs text-muted-foreground"
          onClick={() => setPages((p) => [...p, p.length + 1])}
          disabled={lastPage.isFetching}
        >
          {lastPage.isFetching ? "Loading…" : "Load more comments"}
        </Button>
      )}
    </div>
  );
}
