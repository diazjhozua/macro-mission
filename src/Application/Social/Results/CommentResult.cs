namespace MacroMission.Application.Social.Results;

public sealed record CommentResult(
    string Id,
    string PostId,
    string AuthorId,
    string Text,
    DateTime CreatedAt);
