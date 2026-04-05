namespace MacroMission.Contracts.Meals;

public sealed record DailySummaryResponse(
    DateTime Date,
    MacroTotalsResponse Consumed,
    MacroTotalsResponse? Goal,
    List<MealResponse> Meals);
