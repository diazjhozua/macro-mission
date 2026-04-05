using MacroMission.Domain.Common;
using MongoDB.Bson;

namespace MacroMission.Domain.Meals;

public sealed class Meal : Entity
{
    public ObjectId UserId { get; init; }

    // UTC date truncated to start-of-day — used to group meals for daily summary queries.
    public DateTime Date { get; init; }
    public DateTime LoggedAt { get; init; } = DateTime.UtcNow;
    public MealType MealType { get; set; }
    public List<MealItem> Items { get; init; } = [];

    // Pre-computed on write so daily summary reads are cheap aggregations.
    public MealMacros Totals { get; set; } = new();
}
