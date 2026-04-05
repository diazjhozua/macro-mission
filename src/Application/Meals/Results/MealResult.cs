using MacroMission.Domain.Meals;

namespace MacroMission.Application.Meals.Results;

public sealed record MealResult(
    string Id,
    MealType MealType,
    DateTime Date,
    DateTime LoggedAt,
    List<MealItemResult> Items,
    MacroTotalsResult Totals,
    DateTime CreatedAt);

public sealed record MealItemResult(
    string FoodId,
    string FoodName,
    double Grams,
    MacroTotalsResult Macros);

public sealed record MacroTotalsResult(
    double Calories,
    double Protein,
    double Carbs,
    double Fat,
    double Fiber);

public sealed record DailySummaryResult(
    DateTime Date,
    MacroTotalsResult Consumed,
    MacroTotalsResult? Goal,
    List<MealResult> Meals);
