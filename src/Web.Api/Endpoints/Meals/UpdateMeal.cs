using System.Security.Claims;
using MacroMission.Api.Extensions;
using MacroMission.Api.Infrastructure;
using MacroMission.Application.Common.Messaging;
using MacroMission.Application.Meals.Commands.UpdateMeal;
using MacroMission.Application.Meals.Results;
using MacroMission.Contracts.Meals;
using MacroMission.Domain.Common;
using MacroMission.Domain.Meals;
using MongoDB.Bson;

namespace MacroMission.Api.Endpoints.Meals;

internal sealed class UpdateMeal : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("api/v1/meals/{id}", async (
            string id,
            CreateMealRequest request,
            ClaimsPrincipal user,
            ICommandHandler<UpdateMealCommand, MealResult> handler,
            CancellationToken cancellationToken) =>
        {
            if (!ObjectId.TryParse(id, out ObjectId mealId))
                return Results.BadRequest(new { message = "Invalid meal ID format." });

            if (!Enum.TryParse(request.MealType, ignoreCase: true, out MealType mealType))
                return Results.BadRequest(new { message = $"Invalid meal type. Valid values: {string.Join(", ", Enum.GetNames<MealType>())}" });

            List<UpdateMealItemCommand> items = [];
            foreach (MealItemRequest item in request.Items)
            {
                if (!ObjectId.TryParse(item.FoodId, out ObjectId foodId))
                    return Results.BadRequest(new { message = $"Invalid food ID format: {item.FoodId}" });

                items.Add(new UpdateMealItemCommand(foodId, item.Grams));
            }

            UpdateMealCommand command = new(mealId, user.GetUserId(), mealType, items);
            Result<MealResult> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Meals)
        .WithSummary("Update a logged meal's type and food items.")
        .Produces<MealResult>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status403Forbidden)
        .Produces(StatusCodes.Status400BadRequest)
        .RequireAuthorization();
    }
}
