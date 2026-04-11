// Must match the backend's PostVisibility enum (serialized as strings).
export type PostVisibility = "Public" | "FollowersOnly" | "Private";

export interface PostResult {
  id: string;
  authorId: string;
  mealId: string;
  caption: string | null;
  visibility: PostVisibility;
  likesCount: number;
  commentsCount: number;
  createdAt: string;
  updatedAt: string;
}

export interface CommentResult {
  id: string;
  postId: string;
  authorId: string;
  text: string;
  createdAt: string;
}

// Returned by followers/following and user profile endpoints.
// Does not include email — intentionally kept minimal for public display.
export interface UserSummaryResult {
  id: string;
  nickname: string;
  firstName: string;
  lastName: string;
}

export interface CreatePostRequest {
  mealId: string;
  caption?: string;
  visibility: PostVisibility;
}

export interface UpdatePostRequest {
  caption?: string;
  visibility: PostVisibility;
}
