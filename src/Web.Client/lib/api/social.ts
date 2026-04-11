import { apiClient } from "@/lib/api/client";
import { type UserSummaryResult } from "@/lib/types/social";

export async function getCurrentUser(): Promise<UserSummaryResult> {
  const { data } = await apiClient.get<UserSummaryResult>("/api/v1/users/me");
  return data;
}
