using MacroMission.Application.Common.Interfaces;
using MacroMission.Application.Common.Messaging;
using MacroMission.Application.Foods.Commands.CreateFood;
using MacroMission.Application.Foods.Results;
using MacroMission.Domain.Common;
using MacroMission.Domain.Foods;

namespace MacroMission.Application.Foods.Queries.GetFoodById;

internal sealed class GetFoodByIdQueryHandler(
    IFoodRepository foodRepository) : IQueryHandler<GetFoodByIdQuery, FoodResult>
{
    public async Task<Result<FoodResult>> Handle(
        GetFoodByIdQuery query,
        CancellationToken cancellationToken)
    {
        Food? food = await foodRepository.GetByIdAsync(query.FoodId, cancellationToken);

        if (food is null)
            return Result<FoodResult>.Failure(Error.NotFound("Food.NotFound", "Food not found."));

        // Custom foods are only visible to their owner.
        if (food.IsCustom && food.OwnerId != query.UserId)
            return Result<FoodResult>.Failure(Error.Forbidden("Food.Forbidden", "You do not have access to this food."));

        return Result<FoodResult>.Success(CreateFoodCommandHandler.ToResult(food));
    }
}
