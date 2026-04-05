using MacroMission.Application.Common.Messaging;
using MacroMission.Application.Social.Results;
using MacroMission.Domain.Social;
using MongoDB.Bson;

namespace MacroMission.Application.Social.Commands.UpdatePost;

public sealed record UpdatePostCommand(
    ObjectId PostId,
    ObjectId RequesterId,
    string? Caption,
    PostVisibility Visibility) : ICommand<PostResult>;
