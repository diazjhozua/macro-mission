using MacroMission.Application.Common.Messaging;
using MongoDB.Bson;

namespace MacroMission.Application.Social.Commands.LikePost;

public sealed record LikePostCommand(ObjectId UserId, ObjectId PostId) : ICommand;
