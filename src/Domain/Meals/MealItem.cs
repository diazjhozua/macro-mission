using MongoDB.Bson;

namespace MacroMission.Domain.Meals;

public sealed class MealItem
{
    public ObjectId FoodId { get; init; }

    // Snapshot of the food name at log time — renaming a food later won't corrupt history.
    public string FoodName { get; init; } = string.Empty;
    public double Grams { get; init; }
    public MealMacros Macros { get; init; } = new();
}
