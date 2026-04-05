using MacroMission.Application.Common.Messaging;
using MongoDB.Bson;

namespace MacroMission.Application.Social.Commands.DeletePost;

public sealed record DeletePostCommand(ObjectId PostId, ObjectId RequesterId) : ICommand;
