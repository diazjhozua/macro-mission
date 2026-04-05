namespace MacroMission.Contracts.DailyGoals;

public sealed record UpdateDailyGoalRequest(
    string Name,
    bool IsActive,
    double Calories,
    double Protein,
    double Carbs,
    double Fat,
    double Fiber);
