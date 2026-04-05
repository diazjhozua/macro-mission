using MacroMission.Domain.Common;
using MongoDB.Bson;

namespace MacroMission.Domain.Social;

public sealed class Like : Entity
{
    public ObjectId UserId { get; init; }
    public ObjectId PostId { get; init; }
}
