namespace MacroMission.Application.Foods.Results;

public sealed record FoodResult(
    string Id,
    string Name,
    string? Brand,
    bool IsCustom,
    double Calories,
    double Protein,
    double Carbs,
    double Fat,
    double Fiber,
    DateTime CreatedAt,
    DateTime UpdatedAt);
