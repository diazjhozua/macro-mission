using ErrorOr;
using MacroMission.Application.DailyGoals.Results;
using MediatR;
using MongoDB.Bson;

namespace MacroMission.Application.DailyGoals.Commands.UpdateDailyGoal;

public sealed record UpdateDailyGoalCommand(
    ObjectId GoalId,
    ObjectId UserId,
    string Name,
    bool IsActive,
    double Calories,
    double Protein,
    double Carbs,
    double Fat,
    double Fiber) : IRequest<ErrorOr<DailyGoalResult>>;
