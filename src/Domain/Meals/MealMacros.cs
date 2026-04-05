namespace MacroMission.Domain.Meals;

/// <summary>Computed macros for a meal item or meal total — always derived, never user-entered.</summary>
public sealed class MealMacros
{
    public double Calories { get; init; }
    public double Protein { get; init; }
    public double Carbs { get; init; }
    public double Fat { get; init; }
    public double Fiber { get; init; }

    public static MealMacros operator +(MealMacros a, MealMacros b) => new()
    {
        Calories = a.Calories + b.Calories,
        Protein = a.Protein + b.Protein,
        Carbs = a.Carbs + b.Carbs,
        Fat = a.Fat + b.Fat,
        Fiber = a.Fiber + b.Fiber
    };
}
