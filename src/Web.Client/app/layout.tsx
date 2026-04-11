import type { Metadata } from "next";
import { Geist, Geist_Mono } from "next/font/google";
import { Toaster } from "sonner";
import { QueryProvider } from "@/providers/QueryProvider";
import { AuthProvider } from "@/providers/AuthProvider";
import "./globals.css";

const geistSans = Geist({
  variable: "--font-geist-sans",
  subsets: ["latin"],
});

const geistMono = Geist_Mono({
  variable: "--font-geist-mono",
  subsets: ["latin"],
});

export const metadata: Metadata = {
  title: "Macro Mission",
  description: "Track your nutrition and share your meals.",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html
      lang="en"
      className={`${geistSans.variable} ${geistMono.variable} h-full antialiased`}
    >
      <body className="min-h-full flex flex-col">
        <QueryProvider>
          {/* AuthProvider runs a silent refresh on every cold load to rehydrate
              the access token from the httpOnly cookie. Must be inside
              QueryProvider so child queries fire after the token is ready. */}
          <AuthProvider>
            {children}
          </AuthProvider>
        </QueryProvider>
        {/* Toaster lives outside providers — it doesn't need query context. */}
        <Toaster richColors position="top-right" />
      </body>
    </html>
  );
}
