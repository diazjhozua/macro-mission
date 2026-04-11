import { type NextRequest, NextResponse } from "next/server";

// Routes that don't require authentication.
const PUBLIC_PATHS = ["/login", "/register", "/verify-email"];

export function middleware(request: NextRequest) {
  const { pathname } = request.nextUrl;

  // Let static assets, Next.js internals, and auth API routes through.
  if (
    pathname.startsWith("/_next") ||
    pathname.startsWith("/api/auth") ||
    pathname.startsWith("/public") ||
    PUBLIC_PATHS.some((p) => pathname.startsWith(p))
  ) {
    return NextResponse.next();
  }

  // The refresh token cookie is the coarse session indicator. Its presence
  // means the user has logged in at some point and the token hasn't been
  // explicitly revoked. Actual token validity is enforced by the Axios
  // interceptor when it makes backend calls.
  const hasSession = request.cookies.has("refresh_token");

  if (!hasSession) {
    const loginUrl = new URL("/login", request.url);
    // Preserve the original destination so we can redirect back after login.
    loginUrl.searchParams.set("next", pathname);
    return NextResponse.redirect(loginUrl);
  }

  return NextResponse.next();
}

export const config = {
  // Run on everything except static files and favicon.
  matcher: ["/((?!_next/static|_next/image|favicon.ico).*)"],
};
