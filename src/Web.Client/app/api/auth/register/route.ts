import { type NextRequest, NextResponse } from "next/server";

// Thin passthrough — register returns no tokens, so no cookie handling needed.
export async function POST(request: NextRequest) {
  const body = await request.json();

  const backendRes = await fetch(`${process.env.API_URL}/api/v1/auth/register`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(body),
  });

  const data = await backendRes.json();

  return NextResponse.json(data, { status: backendRes.status });
}
