using MacroMission.Domain.Common;
using MongoDB.Bson;

namespace MacroMission.Domain.DailyGoals;

public sealed class DailyGoal : Entity
{
    public ObjectId UserId { get; init; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }

    // Macros in grams; calories in kcal.
    public double Calories { get; set; }
    public double Protein { get; set; }
    public double Carbs { get; set; }
    public double Fat { get; set; }
    public double Fiber { get; set; }
}
