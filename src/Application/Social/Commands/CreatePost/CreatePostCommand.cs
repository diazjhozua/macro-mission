using MacroMission.Application.Common.Messaging;
using MacroMission.Application.Social.Results;
using MacroMission.Domain.Social;
using MongoDB.Bson;

namespace MacroMission.Application.Social.Commands.CreatePost;

public sealed record CreatePostCommand(
    ObjectId AuthorId,
    ObjectId MealId,
    string? Caption,
    PostVisibility Visibility) : ICommand<PostResult>;
