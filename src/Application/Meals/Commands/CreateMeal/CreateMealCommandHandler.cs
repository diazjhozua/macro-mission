using MacroMission.Application.Common.Interfaces;
using MacroMission.Application.Common.Messaging;
using MacroMission.Application.Meals.Results;
using MacroMission.Domain.Common;
using MacroMission.Domain.Foods;
using MacroMission.Domain.Meals;
using MongoDB.Bson;

namespace MacroMission.Application.Meals.Commands.CreateMeal;

internal sealed class CreateMealCommandHandler(
    IMealRepository mealRepository,
    IFoodRepository foodRepository) : ICommandHandler<CreateMealCommand, MealResult>
{
    public async Task<Result<MealResult>> Handle(
        CreateMealCommand command,
        CancellationToken cancellationToken)
    {
        List<MealItem> items = [];

        foreach (CreateMealItemCommand itemCommand in command.Items)
        {
            Food? food = await foodRepository.GetByIdAsync(itemCommand.FoodId, cancellationToken);

            if (food is null)
                return Result<MealResult>.Failure(
                    Error.NotFound("Meal.FoodNotFound", $"Food {itemCommand.FoodId} not found."));

            // Scale per-100g macros by actual grams eaten.
            double ratio = itemCommand.Grams / 100.0;

            items.Add(new MealItem
            {
                FoodId = food.Id,
                FoodName = food.Name,
                Grams = itemCommand.Grams,
                Macros = new MealMacros
                {
                    Calories = Math.Round(food.Per100g.Calories * ratio, 2),
                    Protein = Math.Round(food.Per100g.Protein * ratio, 2),
                    Carbs = Math.Round(food.Per100g.Carbs * ratio, 2),
                    Fat = Math.Round(food.Per100g.Fat * ratio, 2),
                    Fiber = Math.Round(food.Per100g.Fiber * ratio, 2)
                }
            });
        }

        MealMacros totals = items.Aggregate(new MealMacros(), (acc, item) => acc + item.Macros);

        // Default to start-of-today UTC if no date provided.
        DateTime date = (command.Date?.Date ?? DateTime.UtcNow.Date).ToUniversalTime();

        Meal meal = new()
        {
            UserId = command.UserId,
            MealType = command.MealType,
            Date = date,
            Items = items,
            Totals = totals
        };

        await mealRepository.CreateAsync(meal, cancellationToken);

        return Result<MealResult>.Success(ToResult(meal));
    }

    internal static MealResult ToResult(Meal meal) => new(
        meal.Id.ToString(),
        meal.MealType,
        meal.Date,
        meal.LoggedAt,
        meal.Items.Select(i => new MealItemResult(
            i.FoodId.ToString(),
            i.FoodName,
            i.Grams,
            new MacroTotalsResult(i.Macros.Calories, i.Macros.Protein, i.Macros.Carbs, i.Macros.Fat, i.Macros.Fiber)))
        .ToList(),
        new MacroTotalsResult(
            meal.Totals.Calories,
            meal.Totals.Protein,
            meal.Totals.Carbs,
            meal.Totals.Fat,
            meal.Totals.Fiber),
        meal.CreatedAt);
}
