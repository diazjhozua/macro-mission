using MacroMission.Application.Common.Messaging;
using MongoDB.Bson;

namespace MacroMission.Application.Social.Commands.UnlikePost;

public sealed record UnlikePostCommand(ObjectId UserId, ObjectId PostId) : ICommand;
