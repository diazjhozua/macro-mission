using MacroMission.Application.Common.Messaging;
using MongoDB.Bson;

namespace MacroMission.Application.Social.Commands.Unfollow;

public sealed record UnfollowCommand(ObjectId FollowerId, ObjectId FollowingId) : ICommand;
