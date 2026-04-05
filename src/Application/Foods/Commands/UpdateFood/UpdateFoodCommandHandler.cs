using MacroMission.Application.Common.Interfaces;
using MacroMission.Application.Common.Messaging;
using MacroMission.Application.Foods.Commands.CreateFood;
using MacroMission.Application.Foods.Results;
using MacroMission.Domain.Common;
using MacroMission.Domain.Foods;

namespace MacroMission.Application.Foods.Commands.UpdateFood;

internal sealed class UpdateFoodCommandHandler(
    IFoodRepository foodRepository) : ICommandHandler<UpdateFoodCommand, FoodResult>
{
    public async Task<Result<FoodResult>> Handle(
        UpdateFoodCommand command,
        CancellationToken cancellationToken)
    {
        Food? food = await foodRepository.GetByIdAsync(command.FoodId, cancellationToken);

        if (food is null)
            return Result<FoodResult>.Failure(Error.NotFound("Food.NotFound", "Food not found."));

        // Only the owner can update a custom food; global foods cannot be edited by users.
        if (!food.IsCustom || food.OwnerId != command.RequesterId)
            return Result<FoodResult>.Failure(Error.Forbidden("Food.Forbidden", "You can only edit your own custom foods."));

        food.Name = command.Name;
        food.Brand = command.Brand;
        food.Per100g = new FoodMacros
        {
            Calories = command.Calories,
            Protein = command.Protein,
            Carbs = command.Carbs,
            Fat = command.Fat,
            Fiber = command.Fiber
        };
        food.UpdatedAt = DateTime.UtcNow;

        await foodRepository.UpdateAsync(food, cancellationToken);

        return Result<FoodResult>.Success(CreateFoodCommandHandler.ToResult(food));
    }
}
