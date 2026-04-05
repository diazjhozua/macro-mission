namespace MacroMission.Contracts.Social;

public sealed record CreatePostRequest(string MealId, string? Caption, string Visibility = "Public");
