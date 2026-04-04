using MacroMission.Application.Common.Messaging;
using MacroMission.Application.Foods.Results;
using MongoDB.Bson;

namespace MacroMission.Application.Foods.Commands.UpdateFood;

public sealed record UpdateFoodCommand(
    ObjectId FoodId,
    ObjectId RequesterId,
    string Name,
    string? Brand,
    double Calories,
    double Protein,
    double Carbs,
    double Fat,
    double Fiber) : ICommand<FoodResult>;
