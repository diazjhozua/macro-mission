using MacroMission.Application.Common.Messaging;
using MacroMission.Application.DailyGoals.Results;
using MongoDB.Bson;

namespace MacroMission.Application.DailyGoals.Queries.GetAllDailyGoals;

public sealed record GetAllDailyGoalsQuery(ObjectId UserId) : IQuery<List<DailyGoalResult>>;
