// Standard RFC 7807 problem details — matches what the backend returns on errors.
// The backend's CustomResults.Problem() always produces this shape.
export interface ProblemDetails {
  title: string;
  status: number;
  detail?: string;
  instance?: string;
}

// Validation errors extend problem details with a field → messages map.
// The backend's ValidationError type populates the `errors` dictionary
// from FluentValidation failures, keyed by the property name (camelCase).
export interface ValidationProblemDetails extends ProblemDetails {
  errors: Record<string, string[]>;
}

// Typed error class so callers can distinguish validation failures
// from other API errors without inspecting raw status codes.
export class ApiError extends Error {
  constructor(
    public readonly status: number,
    public readonly problem: ProblemDetails | ValidationProblemDetails,
  ) {
    super(problem.title);
    this.name = "ApiError";
  }

  // Narrows the type so callers get typed access to `errors` without a cast.
  isValidation(): this is ApiError & { problem: ValidationProblemDetails } {
    return "errors" in this.problem;
  }
}
