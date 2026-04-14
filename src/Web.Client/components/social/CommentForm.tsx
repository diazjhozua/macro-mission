"use client";

import { useState } from "react";
import { toast } from "sonner";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { useAddComment } from "@/lib/hooks/useSocial";

interface Props {
  postId: string;
}

export function CommentForm({ postId }: Props) {
  const [text, setText] = useState("");
  const { mutate: addComment, isPending } = useAddComment(postId);

  function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    if (!text.trim()) return;

    addComment(text.trim(), {
      onSuccess: () => setText(""),
      onError:   () => toast.error("Failed to post comment."),
    });
  }

  return (
    <form onSubmit={handleSubmit} className="flex gap-2">
      <Input
        value={text}
        onChange={(e) => setText(e.target.value)}
        placeholder="Add a comment…"
        className="flex-1"
        maxLength={500}
      />
      <Button type="submit" size="sm" disabled={isPending || !text.trim()}>
        {isPending ? "Posting…" : "Post"}
      </Button>
    </form>
  );
}
