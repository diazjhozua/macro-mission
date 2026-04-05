namespace MacroMission.Contracts.Meals;

public sealed record MealResponse(
    string Id,
    string MealType,
    DateTime Date,
    DateTime LoggedAt,
    List<MealItemResponse> Items,
    MacroTotalsResponse Totals,
    DateTime CreatedAt);

public sealed record MealItemResponse(
    string FoodId,
    string FoodName,
    double Grams,
    MacroTotalsResponse Macros);

public sealed record MacroTotalsResponse(
    double Calories,
    double Protein,
    double Carbs,
    double Fat,
    double Fiber);
