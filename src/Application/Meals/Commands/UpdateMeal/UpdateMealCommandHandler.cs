using MacroMission.Application.Common.Interfaces;
using MacroMission.Application.Common.Messaging;
using MacroMission.Application.Meals.Commands.CreateMeal;
using MacroMission.Application.Meals.Results;
using MacroMission.Domain.Common;
using MacroMission.Domain.Foods;
using MacroMission.Domain.Meals;

namespace MacroMission.Application.Meals.Commands.UpdateMeal;

internal sealed class UpdateMealCommandHandler(
    IMealRepository mealRepository,
    IFoodRepository foodRepository) : ICommandHandler<UpdateMealCommand, MealResult>
{
    public async Task<Result<MealResult>> Handle(
        UpdateMealCommand command,
        CancellationToken cancellationToken)
    {
        Meal? meal = await mealRepository.GetByIdAsync(command.MealId, cancellationToken);

        if (meal is null)
            return Result<MealResult>.Failure(Error.NotFound("Meal.NotFound", "Meal not found."));

        if (meal.UserId != command.UserId)
            return Result<MealResult>.Failure(Error.Forbidden("Meal.Forbidden", "You do not have access to this meal."));

        List<MealItem> items = [];

        foreach (UpdateMealItemCommand itemCommand in command.Items)
        {
            Food? food = await foodRepository.GetByIdAsync(itemCommand.FoodId, cancellationToken);

            if (food is null)
                return Result<MealResult>.Failure(
                    Error.NotFound("Meal.FoodNotFound", $"Food {itemCommand.FoodId} not found."));

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

        meal.MealType = command.MealType;
        meal.Items.Clear();
        meal.Items.AddRange(items);
        meal.Totals = items.Aggregate(new MealMacros(), (acc, item) => acc + item.Macros);
        meal.UpdatedAt = DateTime.UtcNow;

        await mealRepository.UpdateAsync(meal, cancellationToken);

        return Result<MealResult>.Success(CreateMealCommandHandler.ToResult(meal));
    }
}
