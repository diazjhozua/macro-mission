// refreshToken is intentionally absent — the Next.js proxy intercepts the
// backend response, strips it, and sets it as an httpOnly cookie instead.
// The browser never sees the refresh token in JS memory.
export interface AuthResponse {
  accessToken: string;
  accessTokenExpiresAt: string; // ISO 8601 datetime string
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  email: string;
  password: string;
  firstName: string;
  lastName: string;
  nickname: string;
}

export interface VerifyEmailRequest {
  token: string;
}
