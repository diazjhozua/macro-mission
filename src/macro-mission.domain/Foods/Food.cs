using MacroMission.Domain.Common;
using MongoDB.Bson;

namespace MacroMission.Domain.Foods;

public sealed class Food : Entity
{
    public string Name { get; set; } = string.Empty;
    public string? Brand { get; set; }

    // Null = global food shared across all users; set = custom food scoped to that user.
    public ObjectId? OwnerId { get; init; }

    public FoodMacros Per100g { get; set; } = new();

    public bool IsCustom => OwnerId.HasValue;
}
