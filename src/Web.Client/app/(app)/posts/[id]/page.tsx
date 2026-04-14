"use client";

import { use } from "react";
import Link from "next/link";
import { ArrowLeft, MessageCircle } from "lucide-react";
import { Card, CardContent, CardHeader } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { LikeButton } from "@/components/social/LikeButton";
import { CommentList } from "@/components/social/CommentList";
import { CommentForm } from "@/components/social/CommentForm";
import { usePost, usePostMeal } from "@/lib/hooks/useSocial";

const MEAL_TYPE_COLORS: Record<string, string> = {
  Breakfast: "bg-amber-50 text-amber-700 border-amber-200",
  Lunch:     "bg-green-50 text-green-700 border-green-200",
  Dinner:    "bg-blue-50 text-blue-700 border-blue-200",
  Snack:     "bg-purple-50 text-purple-700 border-purple-200",
};

export default function PostDetailPage({ params }: { params: Promise<{ id: string }> }) {
  const { id } = use(params);

  const { data: post, isLoading: postLoading } = usePost(id);
  // Fetch the linked meal in parallel — enabled once the post is loaded.
  const { data: meal, isLoading: mealLoading } = usePostMeal(id);

  if (postLoading) {
    return (
      <div className="max-w-xl mx-auto space-y-4">
        <div className="h-8 w-24 bg-muted rounded animate-pulse" />
        <div className="h-48 bg-muted rounded-xl animate-pulse" />
      </div>
    );
  }

  if (!post) {
    return (
      <div className="max-w-xl mx-auto text-center py-16">
        <p className="text-muted-foreground">Post not found or you don't have permission to view it.</p>
        <Link href="/social/explore" className="text-sm underline underline-offset-4 mt-2 inline-block">
          Back to explore
        </Link>
      </div>
    );
  }

  return (
    <div className="max-w-xl mx-auto space-y-4">
      <Link
        href="/social/explore"
        className="inline-flex items-center gap-1.5 text-sm text-muted-foreground hover:text-foreground"
      >
        <ArrowLeft className="h-3.5 w-3.5" />
        Back
      </Link>

      {/* Post card */}
      <Card>
        <CardHeader className="pb-2 pt-4 px-5">
          <div className="flex items-center justify-between">
            <Link href={`/users/${post.authorId}`} className="text-sm font-medium hover:underline">
              @{post.authorId.slice(-6)}
            </Link>
            <span className="text-xs text-muted-foreground">
              {new Date(post.createdAt).toLocaleDateString("en-US", { month: "short", day: "numeric", year: "numeric" })}
            </span>
          </div>
        </CardHeader>

        <CardContent className="px-5 pb-4 space-y-4">
          {post.caption && <p className="text-sm leading-relaxed">{post.caption}</p>}

          {/* Meal breakdown */}
          {mealLoading && <div className="h-24 bg-muted rounded animate-pulse" />}
          {meal && (
            <div className="rounded-md border p-3 space-y-2 bg-muted/30">
              <div className="flex items-center justify-between">
                <Badge variant="outline" className={MEAL_TYPE_COLORS[meal.mealType]}>
                  {meal.mealType}
                </Badge>
                <span className="text-sm font-semibold tabular-nums">
                  {Math.round(meal.totals.calories)} kcal
                </span>
              </div>
              {meal.items.map((item, i) => (
                <div key={i} className="flex items-center justify-between text-sm">
                  <span>{item.foodName}</span>
                  <span className="text-muted-foreground tabular-nums">{item.grams}g</span>
                </div>
              ))}
              <div className="flex gap-3 pt-1 border-t text-xs text-muted-foreground tabular-nums">
                <span>P {Math.round(meal.totals.protein)}g</span>
                <span>C {Math.round(meal.totals.carbs)}g</span>
                <span>F {Math.round(meal.totals.fat)}g</span>
              </div>
            </div>
          )}

          {/* Actions */}
          <div className="flex items-center gap-2 border-t pt-3">
            <LikeButton postId={post.id} likesCount={post.likesCount} />
            <span className="flex items-center gap-1.5 text-sm text-muted-foreground">
              <MessageCircle className="h-4 w-4" />
              {post.commentsCount}
            </span>
          </div>
        </CardContent>
      </Card>

      {/* Comments */}
      <div className="space-y-3">
        <h2 className="text-sm font-medium">Comments</h2>
        <CommentForm postId={post.id} />
        <CommentList postId={post.id} />
      </div>
    </div>
  );
}
