using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace MacroMission.Api.Middleware;

/// <summary>
/// Catches unhandled exceptions and returns RFC 7807 ProblemDetails.
/// Stack traces are hidden in production.
/// </summary>
public sealed class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger,
    IHostEnvironment env)
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception for {Method} {Path}",
                context.Request.Method, context.Request.Path);

            await WriteProblemAsync(context, ex);
        }
    }

    private async Task WriteProblemAsync(HttpContext context, Exception ex)
    {
        ProblemDetails problem = new()
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "An unexpected error occurred.",
            // Only expose the message outside production to avoid leaking internals.
            Detail = env.IsProduction() ? null : ex.Message,
            Instance = context.Request.Path
        };

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        await context.Response.WriteAsync(JsonSerializer.Serialize(problem, JsonOptions));
    }
}
