// Format a Date for display in the dashboard header.
// e.g. "Monday, April 11"
export function formatDisplayDate(date: Date): string {
  return date.toLocaleDateString("en-US", {
    weekday: "long",
    month: "long",
    day: "numeric",
  });
}

// Convert a Date to the ISO date string the API expects for ?date= params.
// e.g. "2026-04-11"
export function toApiDate(date: Date): string {
  return date.toISOString().split("T")[0];
}

// Returns true if two dates fall on the same calendar day.
export function isSameDay(a: Date, b: Date): boolean {
  return toApiDate(a) === toApiDate(b);
}

export function addDays(date: Date, days: number): Date {
  const result = new Date(date);
  result.setDate(result.getDate() + days);
  return result;
}
