using MacroMission.Application.Common.Interfaces;
using MacroMission.Application.Common.Messaging;
using MacroMission.Application.Meals.Commands.CreateMeal;
using MacroMission.Application.Meals.Results;
using MacroMission.Domain.Common;
using MacroMission.Domain.Meals;

namespace MacroMission.Application.Meals.Queries.GetMealsByDate;

internal sealed class GetMealsByDateQueryHandler(
    IMealRepository mealRepository) : IQueryHandler<GetMealsByDateQuery, List<MealResult>>
{
    public async Task<Result<List<MealResult>>> Handle(
        GetMealsByDateQuery query,
        CancellationToken cancellationToken)
    {
        DateTime date = query.Date.Date.ToUniversalTime();

        List<Meal> meals = await mealRepository.GetByDateAsync(
            query.UserId, date, cancellationToken);

        return Result<List<MealResult>>.Success(
            meals.Select(CreateMealCommandHandler.ToResult).ToList());
    }
}
