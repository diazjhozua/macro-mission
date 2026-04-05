namespace MacroMission.Contracts.Meals;

public sealed record CreateMealRequest(
    string MealType,
    DateTime? Date,
    List<MealItemRequest> Items);

public sealed record MealItemRequest(string FoodId, double Grams);
