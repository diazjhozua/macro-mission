namespace MacroMission.Contracts.Social;

public sealed record CommentResponse(
    string Id,
    string PostId,
    string AuthorId,
    string Text,
    DateTime CreatedAt);
