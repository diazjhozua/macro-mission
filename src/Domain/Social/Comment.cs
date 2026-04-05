using MacroMission.Domain.Common;
using MongoDB.Bson;

namespace MacroMission.Domain.Social;

public sealed class Comment : Entity
{
    public ObjectId PostId { get; init; }
    public ObjectId AuthorId { get; init; }
    public string Text { get; set; } = string.Empty;
}
