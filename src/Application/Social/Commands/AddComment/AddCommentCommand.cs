using MacroMission.Application.Common.Messaging;
using MacroMission.Application.Social.Results;
using MongoDB.Bson;

namespace MacroMission.Application.Social.Commands.AddComment;

public sealed record AddCommentCommand(
    ObjectId PostId,
    ObjectId AuthorId,
    string Text) : ICommand<CommentResult>;
