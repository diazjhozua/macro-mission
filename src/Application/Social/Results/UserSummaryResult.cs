namespace MacroMission.Application.Social.Results;

public sealed record UserSummaryResult(
    string Id,
    string Nickname,
    string FirstName,
    string LastName);
