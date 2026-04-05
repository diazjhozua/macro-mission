using System.Security.Claims;
using MacroMission.Api.Extensions;
using MacroMission.Api.Infrastructure;
using MacroMission.Application.Common.Messaging;
using MacroMission.Application.Meals.Commands.CreateMeal;
using MacroMission.Application.Meals.Results;
using MacroMission.Contracts.Meals;
using MacroMission.Domain.Common;
using MacroMission.Domain.Meals;
using MongoDB.Bson;

namespace MacroMission.Api.Endpoints.Meals;

internal sealed class CreateMeal : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/v1/meals", async (
            CreateMealRequest request,
            ClaimsPrincipal user,
            ICommandHandler<CreateMealCommand, MealResult> handler,
            CancellationToken cancellationToken) =>
        {
            if (!Enum.TryParse(request.MealType, ignoreCase: true, out MealType mealType))
                return Results.BadRequest(new { message = $"Invalid meal type. Valid values: {string.Join(", ", Enum.GetNames<MealType>())}" });

            List<CreateMealItemCommand> items = [];
            foreach (MealItemRequest item in request.Items)
            {
                if (!ObjectId.TryParse(item.FoodId, out ObjectId foodId))
                    return Results.BadRequest(new { message = $"Invalid food ID format: {item.FoodId}" });

                items.Add(new CreateMealItemCommand(foodId, item.Grams));
            }

            CreateMealCommand command = new(user.GetUserId(), mealType, request.Date, items);
            Result<MealResult> result = await handler.Handle(command, cancellationToken);

            return result.Match(
                meal => Results.Created($"api/v1/meals/{meal.Id}", meal),
                CustomResults.Problem);
        })
        .WithTags(Tags.Meals)
        .WithSummary("Log a meal with food items and grams eaten.")
        .Produces<MealResult>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .RequireAuthorization();
    }
}
