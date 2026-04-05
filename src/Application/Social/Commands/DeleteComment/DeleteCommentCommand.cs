using MacroMission.Application.Common.Messaging;
using MongoDB.Bson;

namespace MacroMission.Application.Social.Commands.DeleteComment;

public sealed record DeleteCommentCommand(ObjectId CommentId, ObjectId RequesterId) : ICommand;
