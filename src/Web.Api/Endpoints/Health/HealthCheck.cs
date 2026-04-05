namespace MacroMission.Api.Endpoints.Health;

internal sealed class HealthCheck : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v1/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
            .WithTags(Tags.Health)
            .WithSummary("Health check endpoint.")
            .Produces(StatusCodes.Status200OK)
            .AllowAnonymous();
    }
}
