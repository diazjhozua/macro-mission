"use client";

import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import {
  addComment,
  deleteComment,
  deletePost,
  followUser,
  getComments,
  getExplore,
  getFeed,
  getFollowers,
  getFollowing,
  getPostById,
  getPostMeal,
  getUserById,
  getUserPosts,
  likePost,
  unfollowUser,
  unlikePost,
} from "@/lib/api/social";
import { type PostResult } from "@/lib/types/social";

export const socialKeys = {
  feed:      (page: number) => ["social", "feed", page] as const,
  explore:   (page: number) => ["social", "explore", page] as const,
  post:      (id: string)   => ["social", "posts", id] as const,
  postMeal:  (id: string)   => ["social", "posts", id, "meal"] as const,
  comments:  (postId: string, page: number) => ["social", "posts", postId, "comments", page] as const,
  userPosts: (userId: string, page: number) => ["social", "users", userId, "posts", page] as const,
  user:      (userId: string) => ["social", "users", userId] as const,
  followers: (userId: string) => ["social", "users", userId, "followers"] as const,
  following: (userId: string) => ["social", "users", userId, "following"] as const,
};

export function useFeed(page: number) {
  return useQuery({
    queryKey: socialKeys.feed(page),
    queryFn: () => getFeed(page),
  });
}

export function useExplore(page: number) {
  return useQuery({
    queryKey: socialKeys.explore(page),
    queryFn: () => getExplore(page),
  });
}

export function usePost(id: string) {
  return useQuery({
    queryKey: socialKeys.post(id),
    queryFn: () => getPostById(id),
  });
}

export function usePostMeal(postId: string) {
  return useQuery({
    queryKey: socialKeys.postMeal(postId),
    queryFn: () => getPostMeal(postId),
  });
}

export function useComments(postId: string, page: number) {
  return useQuery({
    queryKey: socialKeys.comments(postId, page),
    queryFn: () => getComments(postId, page),
  });
}

export function useUserById(userId: string) {
  return useQuery({
    queryKey: socialKeys.user(userId),
    queryFn: () => getUserById(userId),
    staleTime: 1000 * 60 * 5, // user profiles change rarely
  });
}

export function useUserPosts(userId: string, page: number) {
  return useQuery({
    queryKey: socialKeys.userPosts(userId, page),
    queryFn: () => getUserPosts(userId, page),
  });
}

export function useFollowers(userId: string) {
  return useQuery({
    queryKey: socialKeys.followers(userId),
    queryFn: () => getFollowers(userId),
  });
}

export function useFollowing(userId: string) {
  return useQuery({
    queryKey: socialKeys.following(userId),
    queryFn: () => getFollowing(userId),
  });
}

// ─── Mutations ────────────────────────────────────────────────────────────────

export function useLikePost(postId: string) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: () => likePost(postId),
    onMutate: async () => {
      await queryClient.cancelQueries({ queryKey: socialKeys.post(postId) });
      const prev = queryClient.getQueryData<PostResult>(socialKeys.post(postId));
      if (prev) {
        queryClient.setQueryData(socialKeys.post(postId), {
          ...prev,
          likesCount: prev.likesCount + 1,
        });
      }
      return { prev };
    },
    onError: (_, __, ctx) => {
      if (ctx?.prev) queryClient.setQueryData(socialKeys.post(postId), ctx.prev);
    },
  });
}

export function useUnlikePost(postId: string) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: () => unlikePost(postId),
    onMutate: async () => {
      await queryClient.cancelQueries({ queryKey: socialKeys.post(postId) });
      const prev = queryClient.getQueryData<PostResult>(socialKeys.post(postId));
      if (prev) {
        queryClient.setQueryData(socialKeys.post(postId), {
          ...prev,
          likesCount: Math.max(0, prev.likesCount - 1),
        });
      }
      return { prev };
    },
    onError: (_, __, ctx) => {
      if (ctx?.prev) queryClient.setQueryData(socialKeys.post(postId), ctx.prev);
    },
  });
}

export function useAddComment(postId: string) {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (text: string) => addComment(postId, text),
    onSuccess: () => {
      // Invalidate page 1 so new comment appears at top.
      queryClient.invalidateQueries({ queryKey: socialKeys.comments(postId, 1) });
      // Bump the comment count on the post detail.
      const post = queryClient.getQueryData<PostResult>(socialKeys.post(postId));
      if (post) {
        queryClient.setQueryData(socialKeys.post(postId), {
          ...post,
          commentsCount: post.commentsCount + 1,
        });
      }
    },
  });
}

export function useDeleteComment(postId: string) {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (commentId: string) => deleteComment(postId, commentId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: socialKeys.comments(postId, 1) });
    },
  });
}

export function useDeletePost() {
  return useMutation({ mutationFn: (id: string) => deletePost(id) });
}

export function useFollowUser(userId: string) {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: () => followUser(userId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: socialKeys.followers(userId) });
    },
  });
}

export function useUnfollowUser(userId: string) {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: () => unfollowUser(userId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: socialKeys.followers(userId) });
    },
  });
}
