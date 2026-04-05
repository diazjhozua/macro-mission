using MacroMission.Application.Common.Interfaces;
using MacroMission.Application.Common.Messaging;
using MacroMission.Domain.Common;
using MacroMission.Domain.Meals;

namespace MacroMission.Application.Meals.Commands.DeleteMeal;

internal sealed class DeleteMealCommandHandler(
    IMealRepository mealRepository) : ICommandHandler<DeleteMealCommand>
{
    public async Task<Result> Handle(DeleteMealCommand command, CancellationToken cancellationToken)
    {
        Meal? meal = await mealRepository.GetByIdAsync(command.MealId, cancellationToken);

        if (meal is null)
            return Result.Failure(Error.NotFound("Meal.NotFound", "Meal not found."));

        if (meal.UserId != command.UserId)
            return Result.Failure(Error.Forbidden("Meal.Forbidden", "You do not have access to this meal."));

        await mealRepository.DeleteAsync(command.MealId, cancellationToken);

        return Result.Success();
    }
}
