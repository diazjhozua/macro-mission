namespace MacroMission.Domain.Foods;

/// <summary>Macros stored per 100g — grams eaten scales linearly, math stays consistent.</summary>
public sealed class FoodMacros
{
    public double Calories { get; init; }
    public double Protein { get; init; }
    public double Carbs { get; init; }
    public double Fat { get; init; }
    public double Fiber { get; init; }
}
