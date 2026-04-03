using ErrorOr;
using MacroMission.Application.DailyGoals.Results;
using MediatR;
using MongoDB.Bson;

namespace MacroMission.Application.DailyGoals.Commands.CreateDailyGoal;

public sealed record CreateDailyGoalCommand(
    ObjectId UserId,
    string Name,
    double Calories,
    double Protein,
    double Carbs,
    double Fat,
    double Fiber) : IRequest<ErrorOr<DailyGoalResult>>;
