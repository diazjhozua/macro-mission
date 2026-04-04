namespace MacroMission.Contracts.Foods;

public sealed record UpdateFoodRequest(
    string Name,
    string? Brand,
    double Calories,
    double Protein,
    double Carbs,
    double Fat,
    double Fiber);
