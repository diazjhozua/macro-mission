using ErrorOr;
using MacroMission.Application.DailyGoals.Results;
using MediatR;
using MongoDB.Bson;

namespace MacroMission.Application.DailyGoals.Queries.GetDailyGoalById;

public sealed record GetDailyGoalByIdQuery(ObjectId GoalId, ObjectId UserId) : IRequest<ErrorOr<DailyGoalResult>>;
