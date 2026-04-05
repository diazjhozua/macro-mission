using System.Security.Claims;
using MacroMission.Api.Extensions;
using MacroMission.Api.Infrastructure;
using MacroMission.Application.Common.Messaging;
using MacroMission.Application.Meals.Queries.GetDailySummary;
using MacroMission.Application.Meals.Results;
using MacroMission.Domain.Common;

namespace MacroMission.Api.Endpoints.Meals;

internal sealed class GetDailySummary : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v1/meals/summary", async (
            ClaimsPrincipal user,
            IQueryHandler<GetDailySummaryQuery, DailySummaryResult> handler,
            CancellationToken cancellationToken,
            DateTime? date = null) =>
        {
            DateTime queryDate = date?.Date ?? DateTime.UtcNow.Date;

            Result<DailySummaryResult> result = await handler.Handle(
                new GetDailySummaryQuery(user.GetUserId(), queryDate), cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Meals)
        .WithSummary("Get daily macro summary with totals and progress against active goal.")
        .Produces<DailySummaryResult>(StatusCodes.Status200OK)
        .RequireAuthorization();
    }
}
