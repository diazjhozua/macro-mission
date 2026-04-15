"use client";

import { use } from "react";
import Link from "next/link";
import { ArrowLeft } from "lucide-react";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { FollowButton } from "@/components/users/FollowButton";
import { UserPostsGrid } from "@/components/users/UserPostsGrid";
import { useUserById, useFollowers, useFollowing } from "@/lib/hooks/useSocial";
import { useCurrentUser } from "@/lib/hooks/useCurrentUser";
import { type UserSummaryResult } from "@/lib/types/social";

export default function UserProfilePage({ params }: { params: Promise<{ userId: string }> }) {
  const { userId } = use(params);

  const { data: profile, isLoading: profileLoading } = useUserById(userId);
  const { data: currentUser } = useCurrentUser();
  const { data: followers = [] } = useFollowers(userId);
  const { data: following = [] } = useFollowing(userId);

  const isOwnProfile = currentUser?.id === userId;
  // Check if the current user already follows this person by scanning the list.
  const alreadyFollowing = !!currentUser && followers.some((f) => f.id === currentUser.id);

  if (profileLoading) {
    return (
      <div className="max-w-xl mx-auto space-y-4">
        <div className="h-8 w-24 bg-muted rounded animate-pulse" />
        <div className="h-24 bg-muted rounded-xl animate-pulse" />
      </div>
    );
  }

  if (!profile) {
    return (
      <div className="max-w-xl mx-auto text-center py-16">
        <p className="text-muted-foreground text-sm">User not found.</p>
        <Link href="/social/explore" className="text-sm underline underline-offset-4 mt-2 inline-block">
          Back to explore
        </Link>
      </div>
    );
  }

  return (
    <div className="max-w-xl mx-auto space-y-5">
      <Link
        href="/social/explore"
        className="inline-flex items-center gap-1.5 text-sm text-muted-foreground hover:text-foreground"
      >
        <ArrowLeft className="h-3.5 w-3.5" />
        Back
      </Link>

      {/* Profile header */}
      <div className="flex items-start justify-between">
        <div>
          <h1 className="text-xl font-semibold">
            {profile.firstName} {profile.lastName}
          </h1>
          <p className="text-sm text-muted-foreground">@{profile.nickname}</p>
          <div className="flex gap-4 mt-2 text-sm text-muted-foreground">
            <span><strong className="text-foreground">{followers.length}</strong> followers</span>
            <span><strong className="text-foreground">{following.length}</strong> following</span>
          </div>
        </div>

        {/* Don't show the follow button on the user's own profile. */}
        {!isOwnProfile && (
          <FollowButton userId={userId} initiallyFollowing={alreadyFollowing} />
        )}
      </div>

      {/* Posts + followers/following tabs */}
      <Tabs defaultValue="posts">
        <TabsList>
          <TabsTrigger value="posts">Posts</TabsTrigger>
          <TabsTrigger value="followers">Followers</TabsTrigger>
          <TabsTrigger value="following">Following</TabsTrigger>
        </TabsList>

        <TabsContent value="posts" className="mt-4">
          <UserPostsGrid userId={userId} />
        </TabsContent>

        <TabsContent value="followers" className="mt-4">
          <UserList users={followers} emptyMessage="No followers yet." />
        </TabsContent>

        <TabsContent value="following" className="mt-4">
          <UserList users={following} emptyMessage="Not following anyone yet." />
        </TabsContent>
      </Tabs>
    </div>
  );
}

function UserList({ users, emptyMessage }: { users: UserSummaryResult[]; emptyMessage: string }) {
  if (users.length === 0) {
    return <p className="text-sm text-muted-foreground text-center py-8">{emptyMessage}</p>;
  }

  return (
    <div className="space-y-2">
      {users.map((user) => (
        <Link
          key={user.id}
          href={`/users/${user.id}`}
          className="flex items-center gap-3 p-3 rounded-lg hover:bg-muted transition-colors"
        >
          <div className="h-8 w-8 rounded-full bg-neutral-200 flex items-center justify-center text-xs font-medium shrink-0">
            {user.nickname[0]?.toUpperCase()}
          </div>
          <div>
            <p className="text-sm font-medium">{user.firstName} {user.lastName}</p>
            <p className="text-xs text-muted-foreground">@{user.nickname}</p>
          </div>
        </Link>
      ))}
    </div>
  );
}
