using MacroMission.Application.Common.Interfaces;
using MacroMission.Application.Common.Messaging;
using MacroMission.Application.Meals.Commands.CreateMeal;
using MacroMission.Application.Meals.Results;
using MacroMission.Domain.Common;
using MacroMission.Domain.Meals;

namespace MacroMission.Application.Meals.Queries.GetMealById;

internal sealed class GetMealByIdQueryHandler(
    IMealRepository mealRepository) : IQueryHandler<GetMealByIdQuery, MealResult>
{
    public async Task<Result<MealResult>> Handle(
        GetMealByIdQuery query,
        CancellationToken cancellationToken)
    {
        Meal? meal = await mealRepository.GetByIdAsync(query.MealId, cancellationToken);

        if (meal is null)
            return Result<MealResult>.Failure(
                Error.NotFound("Meal.NotFound", "Meal not found."));

        if (meal.UserId != query.UserId)
            return Result<MealResult>.Failure(
                Error.Forbidden("Meal.Forbidden", "You do not have access to this meal."));

        return Result<MealResult>.Success(CreateMealCommandHandler.ToResult(meal));
    }
}
