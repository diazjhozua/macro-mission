using MacroMission.Application.Common.Interfaces;
using MacroMission.Application.Common.Messaging;
using MacroMission.Application.Foods.Results;
using MacroMission.Domain.Common;
using MacroMission.Domain.Foods;

namespace MacroMission.Application.Foods.Commands.CreateFood;

internal sealed class CreateFoodCommandHandler(
    IFoodRepository foodRepository) : ICommandHandler<CreateFoodCommand, FoodResult>
{
    public async Task<Result<FoodResult>> Handle(
        CreateFoodCommand command,
        CancellationToken cancellationToken)
    {
        Food food = new()
        {
            OwnerId = command.OwnerId,
            Name = command.Name,
            Brand = command.Brand,
            Per100g = new FoodMacros
            {
                Calories = command.Calories,
                Protein = command.Protein,
                Carbs = command.Carbs,
                Fat = command.Fat,
                Fiber = command.Fiber
            }
        };

        await foodRepository.CreateAsync(food, cancellationToken);

        return Result<FoodResult>.Success(ToResult(food));
    }

    internal static FoodResult ToResult(Food food) => new(
        food.Id.ToString(),
        food.Name,
        food.Brand,
        food.IsCustom,
        food.Per100g.Calories,
        food.Per100g.Protein,
        food.Per100g.Carbs,
        food.Per100g.Fat,
        food.Per100g.Fiber,
        food.CreatedAt,
        food.UpdatedAt);
}
