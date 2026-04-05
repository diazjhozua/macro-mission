namespace MacroMission.Application.DailyGoals.Results;

public sealed record DailyGoalResult(
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
