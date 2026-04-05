using MacroMission.Application.Common.Interfaces;
using MacroMission.Application.Common.Messaging;
using MacroMission.Application.Meals.Commands.CreateMeal;
using MacroMission.Application.Meals.Results;
using MacroMission.Domain.Common;
using MacroMission.Domain.DailyGoals;
using MacroMission.Domain.Meals;

namespace MacroMission.Application.Meals.Queries.GetDailySummary;

internal sealed class GetDailySummaryQueryHandler(
    IMealRepository mealRepository,
    IDailyGoalRepository dailyGoalRepository) : IQueryHandler<GetDailySummaryQuery, DailySummaryResult>
{
    public async Task<Result<DailySummaryResult>> Handle(
        GetDailySummaryQuery query,
        CancellationToken cancellationToken)
    {
        DateTime date = query.Date.Date.ToUniversalTime();

        List<Meal> meals = await mealRepository.GetByDateAsync(
            query.UserId, date, cancellationToken);

        // Sum totals across all meals for the day.
        MealMacros consumed = meals.Aggregate(
            new MealMacros(),
            (acc, meal) => acc + meal.Totals);

        // Include active goal if set — null means the user hasn't configured one yet.
        DailyGoal? activeGoal = await dailyGoalRepository
            .GetActiveByUserIdAsync(query.UserId, cancellationToken);

        MacroTotalsResult? goalResult = activeGoal is null ? null : new MacroTotalsResult(
            activeGoal.Calories,
            activeGoal.Protein,
            activeGoal.Carbs,
            activeGoal.Fat,
            activeGoal.Fiber);

        DailySummaryResult summary = new(
            date,
            new MacroTotalsResult(
                consumed.Calories,
                consumed.Protein,
                consumed.Carbs,
                consumed.Fat,
                consumed.Fiber),
            goalResult,
            meals.Select(CreateMealCommandHandler.ToResult).ToList());

        return Result<DailySummaryResult>.Success(summary);
    }
}
