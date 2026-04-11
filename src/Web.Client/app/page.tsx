import { redirect } from "next/navigation";

// Root has no content — send to dashboard.
// Middleware will redirect to /login if there's no active session.
export default function RootPage() {
  redirect("/dashboard");
}
