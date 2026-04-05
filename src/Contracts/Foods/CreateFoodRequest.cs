namespace MacroMission.Contracts.Foods;

public sealed record CreateFoodRequest(
    string Name,
    string? Brand,
    double Calories,
    double Protein,
    double Carbs,
    double Fat,
    double Fiber);
