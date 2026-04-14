import Link from "next/link";
import { Heart, MessageCircle } from "lucide-react";
import { Card, CardContent, CardHeader } from "@/components/ui/card";
import { type PostResult } from "@/lib/types/social";

interface Props {
  post: PostResult;
}

// Formats a UTC ISO string as a relative time string (e.g. "3 hours ago").
function relativeTime(iso: string): string {
  const diff = Date.now() - new Date(iso).getTime();
  const minutes = Math.floor(diff / 60_000);
  if (minutes < 1)  return "just now";
  if (minutes < 60) return `${minutes}m ago`;
  const hours = Math.floor(minutes / 60);
  if (hours < 24)   return `${hours}h ago`;
  const days = Math.floor(hours / 24);
  if (days < 7)     return `${days}d ago`;
  return new Date(iso).toLocaleDateString("en-US", { month: "short", day: "numeric" });
}

export function PostCard({ post }: Props) {
  return (
    <Card className="hover:shadow-sm transition-shadow">
      <CardHeader className="pb-2 pt-4 px-5">
        <div className="flex items-center justify-between">
          {/* Author links to their profile page. */}
          <Link
            href={`/users/${post.authorId}`}
            className="text-sm font-medium hover:underline underline-offset-4"
          >
            @{post.authorId.slice(-6)}
          </Link>
          <span className="text-xs text-muted-foreground">{relativeTime(post.createdAt)}</span>
        </div>
      </CardHeader>

      <CardContent className="px-5 pb-4 space-y-3">
        {post.caption && (
          <p className="text-sm leading-relaxed">{post.caption}</p>
        )}

        <div className="flex items-center justify-between">
          <div className="flex items-center gap-4 text-sm text-muted-foreground">
            <span className="flex items-center gap-1.5">
              <Heart className="h-4 w-4" />
              {post.likesCount}
            </span>
            <span className="flex items-center gap-1.5">
              <MessageCircle className="h-4 w-4" />
              {post.commentsCount}
            </span>
          </div>

          <Link
            href={`/posts/${post.id}`}
            className="text-xs text-muted-foreground hover:text-foreground underline-offset-4 hover:underline"
          >
            View post
          </Link>
        </div>
      </CardContent>
    </Card>
  );
}
