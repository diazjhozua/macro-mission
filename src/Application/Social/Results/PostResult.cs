using MacroMission.Domain.Social;

namespace MacroMission.Application.Social.Results;

public sealed record PostResult(
    string Id,
    string AuthorId,
    string MealId,
    string? Caption,
    PostVisibility Visibility,
    int LikesCount,
    int CommentsCount,
    DateTime CreatedAt,
    DateTime UpdatedAt);
