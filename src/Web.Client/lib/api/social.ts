import { apiClient } from "@/lib/api/client";
import { type MealResult } from "@/lib/types/meal";
import {
  type CommentResult,
  type CreatePostRequest,
  type PostResult,
  type UpdatePostRequest,
  type UserSummaryResult,
} from "@/lib/types/social";

export async function getCurrentUser(): Promise<UserSummaryResult> {
  const { data } = await apiClient.get<UserSummaryResult>("/api/v1/users/me");
  return data;
}

export async function getUserById(userId: string): Promise<UserSummaryResult> {
  const { data } = await apiClient.get<UserSummaryResult>(`/api/v1/users/${userId}`);
  return data;
}

// ─── Posts ────────────────────────────────────────────────────────────────────

export async function getFeed(page: number, pageSize = 20): Promise<PostResult[]> {
  const { data } = await apiClient.get<PostResult[]>("/api/v1/feed", { params: { page, pageSize } });
  return data;
}

export async function getExplore(page: number, pageSize = 20): Promise<PostResult[]> {
  const { data } = await apiClient.get<PostResult[]>("/api/v1/explore", { params: { page, pageSize } });
  return data;
}

export async function getPostById(id: string): Promise<PostResult> {
  const { data } = await apiClient.get<PostResult>(`/api/v1/posts/${id}`);
  return data;
}

export async function getUserPosts(userId: string, page: number, pageSize = 20): Promise<PostResult[]> {
  const { data } = await apiClient.get<PostResult[]>(`/api/v1/users/${userId}/posts`, {
    params: { page, pageSize },
  });
  return data;
}

export async function createPost(payload: CreatePostRequest): Promise<PostResult> {
  const { data } = await apiClient.post<PostResult>("/api/v1/posts", payload);
  return data;
}

export async function updatePost(id: string, payload: UpdatePostRequest): Promise<PostResult> {
  const { data } = await apiClient.put<PostResult>(`/api/v1/posts/${id}`, payload);
  return data;
}

export async function deletePost(id: string): Promise<void> {
  await apiClient.delete(`/api/v1/posts/${id}`);
}

// Returns the meal linked to a post, gated by the post's visibility rules.
export async function getPostMeal(postId: string): Promise<MealResult> {
  const { data } = await apiClient.get<MealResult>(`/api/v1/posts/${postId}/meal`);
  return data;
}

// ─── Likes ────────────────────────────────────────────────────────────────────

export async function likePost(id: string): Promise<void> {
  await apiClient.post(`/api/v1/posts/${id}/likes`);
}

export async function unlikePost(id: string): Promise<void> {
  await apiClient.delete(`/api/v1/posts/${id}/likes`);
}

// ─── Comments ─────────────────────────────────────────────────────────────────

export async function getComments(postId: string, page: number, pageSize = 20): Promise<CommentResult[]> {
  const { data } = await apiClient.get<CommentResult[]>(`/api/v1/posts/${postId}/comments`, {
    params: { page, pageSize },
  });
  return data;
}

export async function addComment(postId: string, text: string): Promise<CommentResult> {
  const { data } = await apiClient.post<CommentResult>(`/api/v1/posts/${postId}/comments`, { text });
  return data;
}

export async function deleteComment(postId: string, commentId: string): Promise<void> {
  await apiClient.delete(`/api/v1/posts/${postId}/comments/${commentId}`);
}

// ─── Follow ───────────────────────────────────────────────────────────────────

export async function followUser(userId: string): Promise<void> {
  await apiClient.post(`/api/v1/users/${userId}/follow`);
}

export async function unfollowUser(userId: string): Promise<void> {
  await apiClient.delete(`/api/v1/users/${userId}/follow`);
}

export async function getFollowers(userId: string): Promise<UserSummaryResult[]> {
  const { data } = await apiClient.get<UserSummaryResult[]>(`/api/v1/users/${userId}/followers`);
  return data;
}

export async function getFollowing(userId: string): Promise<UserSummaryResult[]> {
  const { data } = await apiClient.get<UserSummaryResult[]>(`/api/v1/users/${userId}/following`);
  return data;
}
