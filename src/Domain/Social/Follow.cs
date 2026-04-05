using MacroMission.Domain.Common;
using MongoDB.Bson;

namespace MacroMission.Domain.Social;

public sealed class Follow : Entity
{
    public ObjectId FollowerId { get; init; }
    public ObjectId FollowingId { get; init; }
}
