using MacroMission.Application.Common.Messaging;
using MongoDB.Bson;

namespace MacroMission.Application.Social.Commands.Follow;

public sealed record FollowCommand(ObjectId FollowerId, ObjectId FollowingId) : ICommand;
