import { cookies } from "next/headers";
import { type NextRequest, NextResponse } from "next/server";

export async function POST(request: NextRequest) {
  const body = await request.json();

  const backendRes = await fetch(`${process.env.API_URL}/api/v1/auth/login`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(body),
  });

  const data = await backendRes.json();

  if (!backendRes.ok) {
    return NextResponse.json(data, { status: backendRes.status });
  }

  // Strip the refresh token before it reaches the browser.
  // Set it as an httpOnly cookie so JS can never read it.
  const cookieStore = await cookies();
  cookieStore.set("refresh_token", data.refreshToken, {
    httpOnly: true,
    secure: process.env.NODE_ENV === "production",
    sameSite: "lax",
    // Scoped to only the refresh endpoint — the browser won't send this
    // cookie on any other request, reducing the CSRF surface area.
    path: "/api/auth/refresh",
    maxAge: 60 * 60 * 24 * 7, // 7 days — matches the backend token TTL
  });

  return NextResponse.json({
    accessToken: data.accessToken,
    accessTokenExpiresAt: data.accessTokenExpiresAt,
  });
}
