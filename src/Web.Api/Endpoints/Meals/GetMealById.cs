using System.Security.Claims;
using MacroMission.Api.Extensions;
using MacroMission.Api.Infrastructure;
using MacroMission.Application.Common.Messaging;
using MacroMission.Application.Meals.Queries.GetMealById;
using MacroMission.Application.Meals.Results;
using MacroMission.Domain.Common;
using MongoDB.Bson;

namespace MacroMission.Api.Endpoints.Meals;

internal sealed class GetMealById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v1/meals/{id}", async (
            string id,
            ClaimsPrincipal user,
            IQueryHandler<GetMealByIdQuery, MealResult> handler,
            CancellationToken cancellationToken) =>
        {
            if (!ObjectId.TryParse(id, out ObjectId mealId))
                return Results.BadRequest(new { message = "Invalid meal ID format." });

            Result<MealResult> result = await handler.Handle(
                new GetMealByIdQuery(mealId, user.GetUserId()), cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Meals)
        .WithSummary("Get a meal by ID.")
        .Produces<MealResult>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status403Forbidden)
        .Produces(StatusCodes.Status404NotFound)
        .RequireAuthorization();
    }
}
