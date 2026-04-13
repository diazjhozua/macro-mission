import { cookies } from "next/headers";
import { type NextRequest, NextResponse } from "next/server";

const COOKIE_OPTIONS = {
  httpOnly: true,
  secure: process.env.NODE_ENV === "production",
  sameSite: "lax" as const,
  path: "/",
  maxAge: 60 * 60 * 24 * 7,
};

export async function POST(_request: NextRequest) {
  const cookieStore = await cookies();
  const refreshToken = cookieStore.get("refresh_token")?.value;

  if (!refreshToken) {
    return NextResponse.json({ title: "Unauthorized", status: 401 }, { status: 401 });
  }

  const backendRes = await fetch(`${process.env.API_URL}/api/v1/auth/refresh-token`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    // The backend expects the token in the request body, not as a cookie.
    // We bridge that here — the browser never touches the token value.
    body: JSON.stringify({ refreshToken }),
  });

  const data = await backendRes.json();

  if (!backendRes.ok) {
    // Only clear the cookie on a genuine auth failure (401).
    // A 429 rate limit or 500 server error is transient — clearing the cookie
    // would permanently log the user out for a temporary problem.
    if (backendRes.status === 401) {
      cookieStore.set("refresh_token", "", { ...COOKIE_OPTIONS, maxAge: 0 });
    }

    return NextResponse.json(data, { status: backendRes.status });
  }

  // Rotate the cookie with the new refresh token.
  cookieStore.set("refresh_token", data.refreshToken, COOKIE_OPTIONS);

  return NextResponse.json({
    accessToken: data.accessToken,
    accessTokenExpiresAt: data.accessTokenExpiresAt,
  });
}
