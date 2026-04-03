using MongoDB.Bson;

namespace MacroMission.Domain.Common;

/// <summary>Base for all domain entities stored in Mongo.</summary>
public abstract class Entity
{
    // ObjectId is sortable by creation time and native to Mongo — no UUID overhead.
    public ObjectId Id { get; init; } = ObjectId.GenerateNewId();

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
