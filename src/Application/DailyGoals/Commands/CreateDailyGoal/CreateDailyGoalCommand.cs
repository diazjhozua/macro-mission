using MacroMission.Application.Common.Messaging;
using MacroMission.Application.DailyGoals.Results;
using MongoDB.Bson;

namespace MacroMission.Application.DailyGoals.Commands.CreateDailyGoal;

public sealed record CreateDailyGoalCommand(
    ObjectId UserId,
    string Name,
    double Calories,
    double Protein,
    double Carbs,
    double Fat,
    double Fiber) : ICommand<DailyGoalResult>;
