using ErrorOr;
using MacroMission.Application.DailyGoals.Results;
using MediatR;
using MongoDB.Bson;

namespace MacroMission.Application.DailyGoals.Queries.GetAllDailyGoals;

public sealed record GetAllDailyGoalsQuery(ObjectId UserId) : IRequest<ErrorOr<List<DailyGoalResult>>>;
