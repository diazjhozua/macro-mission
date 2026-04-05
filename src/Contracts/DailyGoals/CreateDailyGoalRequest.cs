namespace MacroMission.Contracts.DailyGoals;

public sealed record CreateDailyGoalRequest(
    string Name,
    double Calories,
    double Protein,
    double Carbs,
    double Fat,
    double Fiber);
