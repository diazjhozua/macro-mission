using ErrorOr;
using MediatR;
using MongoDB.Bson;

namespace MacroMission.Application.DailyGoals.Commands.DeleteDailyGoal;

public sealed record DeleteDailyGoalCommand(ObjectId GoalId, ObjectId UserId) : IRequest<ErrorOr<Deleted>>;
