using System.Security.Claims;
using MacroMission.Api.Extensions;
using MacroMission.Api.Infrastructure;
using MacroMission.Application.Common.Messaging;
using MacroMission.Application.Meals.Queries.GetMealsByDate;
using MacroMission.Application.Meals.Results;
using MacroMission.Domain.Common;

namespace MacroMission.Api.Endpoints.Meals;

internal sealed class GetMealsByDate : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v1/meals", async (
            ClaimsPrincipal user,
            IQueryHandler<GetMealsByDateQuery, List<MealResult>> handler,
            CancellationToken cancellationToken,
            DateTime? date = null) =>
        {
            DateTime queryDate = date?.Date ?? DateTime.UtcNow.Date;

            Result<List<MealResult>> result = await handler.Handle(
                new GetMealsByDateQuery(user.GetUserId(), queryDate), cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Meals)
        .WithSummary("Get all meals for a specific date. Defaults to today.")
        .Produces<List<MealResult>>(StatusCodes.Status200OK)
        .RequireAuthorization();
    }
}
