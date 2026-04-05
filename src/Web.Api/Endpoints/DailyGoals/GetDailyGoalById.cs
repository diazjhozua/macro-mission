using System.Security.Claims;
using MacroMission.Api.Extensions;
using MacroMission.Api.Infrastructure;
using MacroMission.Application.Common.Messaging;
using MacroMission.Application.DailyGoals.Queries.GetDailyGoalById;
using MacroMission.Application.DailyGoals.Results;
using MacroMission.Domain.Common;
using MongoDB.Bson;

namespace MacroMission.Api.Endpoints.DailyGoals;

internal sealed class GetDailyGoalById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v1/dailygoals/{id}", async (
            string id,
            ClaimsPrincipal user,
            IQueryHandler<GetDailyGoalByIdQuery, DailyGoalResult> handler,
            CancellationToken cancellationToken) =>
        {
            if (!ObjectId.TryParse(id, out ObjectId goalId))
                return Results.BadRequest(new { message = "Invalid goal ID format." });

            Result<DailyGoalResult> result = await handler.Handle(
                new GetDailyGoalByIdQuery(goalId, user.GetUserId()), cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.DailyGoals)
        .WithSummary("Get a specific daily goal by ID.")
        .Produces<DailyGoalResult>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status403Forbidden)
        .RequireAuthorization();
    }
}
