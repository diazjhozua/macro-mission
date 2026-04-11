import { create } from "zustand";

interface AuthState {
  accessToken: string | null;
  expiresAt: Date | null;

  setTokens: (accessToken: string, expiresAt: string) => void;
  clear: () => void;
  isAuthenticated: () => boolean;
}

export const useAuthStore = create<AuthState>((set, get) => ({
  accessToken: null,
  expiresAt: null,

  setTokens: (accessToken, expiresAt) => {
    set({
      accessToken,
      expiresAt: new Date(expiresAt),
    });
  },

  clear: () => set({ accessToken: null, expiresAt: null }),

  // Treats the token as valid if it exists and hasn't passed its expiry.
  // The interceptor handles actual re-auth when the backend returns 401.
  isAuthenticated: () => {
    const { accessToken, expiresAt } = get();
    return !!accessToken && !!expiresAt && expiresAt > new Date();
  },
}));
