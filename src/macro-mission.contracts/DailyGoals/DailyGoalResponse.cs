namespace MacroMission.Contracts.DailyGoals;

public sealed record DailyGoalResponse(
    string Id,
    string Name,
    bool IsActive,
    double Calories,
    double Protein,
    double Carbs,
    double Fat,
    double Fiber,
    DateTime CreatedAt,
    DateTime UpdatedAt);
