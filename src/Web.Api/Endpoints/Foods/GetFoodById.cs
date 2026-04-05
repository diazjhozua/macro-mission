using System.Security.Claims;
using MacroMission.Api.Extensions;
using MacroMission.Api.Infrastructure;
using MacroMission.Application.Common.Messaging;
using MacroMission.Application.Foods.Queries.GetFoodById;
using MacroMission.Application.Foods.Results;
using MacroMission.Domain.Common;
using MongoDB.Bson;

namespace MacroMission.Api.Endpoints.Foods;

internal sealed class GetFoodById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v1/foods/{id}", async (
            string id,
            ClaimsPrincipal user,
            IQueryHandler<GetFoodByIdQuery, FoodResult> handler,
            CancellationToken cancellationToken) =>
        {
            if (!ObjectId.TryParse(id, out ObjectId foodId))
                return Results.BadRequest(new { message = "Invalid food ID format." });

            Result<FoodResult> result = await handler.Handle(
                new GetFoodByIdQuery(foodId, user.GetUserId()), cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Foods)
        .WithSummary("Get a food by ID.")
        .Produces<FoodResult>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status403Forbidden)
        .RequireAuthorization();
    }
}
