using System.Security.Claims;
using MacroMission.Api.Extensions;
using MacroMission.Api.Infrastructure;
using MacroMission.Application.Common.Messaging;
using MacroMission.Application.Foods.Commands.UpdateFood;
using MacroMission.Application.Foods.Results;
using MacroMission.Contracts.Foods;
using MacroMission.Domain.Common;
using MongoDB.Bson;

namespace MacroMission.Api.Endpoints.Foods;

internal sealed class UpdateFood : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("api/v1/foods/{id}", async (
            string id,
            UpdateFoodRequest request,
            ClaimsPrincipal user,
            ICommandHandler<UpdateFoodCommand, FoodResult> handler,
            CancellationToken cancellationToken) =>
        {
            if (!ObjectId.TryParse(id, out ObjectId foodId))
                return Results.BadRequest(new { message = "Invalid food ID format." });

            UpdateFoodCommand command = new(
                foodId,
                user.GetUserId(),
                request.Name,
                request.Brand,
                request.Calories,
                request.Protein,
                request.Carbs,
                request.Fat,
                request.Fiber);

            Result<FoodResult> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Foods)
        .WithSummary("Update your custom food.")
        .Produces<FoodResult>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status403Forbidden)
        .Produces(StatusCodes.Status400BadRequest)
        .RequireAuthorization();
    }
}
