namespace MacroMission.Contracts.Social;

public sealed record PostResponse(
    string Id,
    string AuthorId,
    string MealId,
    string? Caption,
    string Visibility,
    int LikesCount,
    int CommentsCount,
    DateTime CreatedAt,
    DateTime UpdatedAt);
