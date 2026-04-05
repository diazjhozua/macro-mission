using System.Security.Claims;
using MacroMission.Api.Extensions;
using MacroMission.Api.Infrastructure;
using MacroMission.Application.Common.Messaging;
using MacroMission.Application.DailyGoals.Queries.GetAllDailyGoals;
using MacroMission.Application.DailyGoals.Results;
using MacroMission.Domain.Common;

namespace MacroMission.Api.Endpoints.DailyGoals;

internal sealed class GetAllDailyGoals : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v1/dailygoals", async (
            ClaimsPrincipal user,
            IQueryHandler<GetAllDailyGoalsQuery, List<DailyGoalResult>> handler,
            CancellationToken cancellationToken) =>
        {
            Result<List<DailyGoalResult>> result = await handler.Handle(
                new GetAllDailyGoalsQuery(user.GetUserId()), cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.DailyGoals)
        .WithSummary("Get all daily goals for the authenticated user.")
        .Produces<List<DailyGoalResult>>(StatusCodes.Status200OK)
        .RequireAuthorization();
    }
}
