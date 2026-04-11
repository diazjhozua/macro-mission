"use client";

import Link from "next/link";
import { useEffect, useRef } from "react";
import { useSearchParams } from "next/navigation";
import { Loader2 } from "lucide-react";
import { Card, CardDescription, CardFooter, CardHeader, CardTitle } from "@/components/ui/card";
import { useVerifyEmail } from "@/lib/hooks/useAuth";

export default function VerifyEmailPage() {
  const searchParams = useSearchParams();
  const token = searchParams.get("token");

  const { mutate: verify, isPending, isSuccess, isError } = useVerifyEmail();

  // Trigger verification once on mount. The ref guard prevents the double
  // invocation that React 18 strict mode causes in development.
  const attempted = useRef(false);
  useEffect(() => {
    if (attempted.current || !token) return;
    attempted.current = true;
    verify(token);
  }, [token, verify]);

  if (!token) {
    return (
      <Card>
        <CardHeader>
          <CardTitle>Invalid link</CardTitle>
          <CardDescription>This verification link is missing a token. Please check your email and try again.</CardDescription>
        </CardHeader>
        <CardFooter>
          <Link href="/login" className="text-sm underline underline-offset-4 hover:text-foreground text-muted-foreground">
            Back to sign in
          </Link>
        </CardFooter>
      </Card>
    );
  }

  if (isPending) {
    return (
      <Card>
        <CardHeader>
          <div className="flex items-center gap-2">
            <Loader2 className="h-4 w-4 animate-spin text-muted-foreground" />
            <CardTitle>Verifying…</CardTitle>
          </div>
          <CardDescription>Just a moment while we confirm your email.</CardDescription>
        </CardHeader>
      </Card>
    );
  }

  if (isSuccess) {
    return (
      <Card>
        <CardHeader>
          <CardTitle>Email verified</CardTitle>
          <CardDescription>Your account is active. Redirecting you to sign in…</CardDescription>
        </CardHeader>
      </Card>
    );
  }

  if (isError) {
    return (
      <Card>
        <CardHeader>
          <CardTitle>Verification failed</CardTitle>
          <CardDescription>This link may have expired or already been used. Request a new one by registering again.</CardDescription>
        </CardHeader>
        <CardFooter>
          <Link href="/register" className="text-sm underline underline-offset-4 hover:text-foreground text-muted-foreground">
            Back to register
          </Link>
        </CardFooter>
      </Card>
    );
  }

  return null;
}
