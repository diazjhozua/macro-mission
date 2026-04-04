using MacroMission.Application.Common.Interfaces;
using MacroMission.Application.Common.Messaging;
using MacroMission.Domain.Common;
using MacroMission.Domain.Foods;

namespace MacroMission.Application.Foods.Commands.DeleteFood;

internal sealed class DeleteFoodCommandHandler(
    IFoodRepository foodRepository) : ICommandHandler<DeleteFoodCommand>
{
    public async Task<Result> Handle(DeleteFoodCommand command, CancellationToken cancellationToken)
    {
        Food? food = await foodRepository.GetByIdAsync(command.FoodId, cancellationToken);

        if (food is null)
            return Result.Failure(Error.NotFound("Food.NotFound", "Food not found."));

        // Only the owner can delete a custom food; global foods are managed at the system level.
        if (!food.IsCustom || food.OwnerId != command.RequesterId)
            return Result.Failure(Error.Forbidden("Food.Forbidden", "You can only delete your own custom foods."));

        await foodRepository.DeleteAsync(command.FoodId, cancellationToken);

        return Result.Success();
    }
}
