using MacroMission.Application.Common.Messaging;
using MacroMission.Application.DailyGoals.Results;
using MongoDB.Bson;

namespace MacroMission.Application.DailyGoals.Queries.GetDailyGoalById;

public sealed record GetDailyGoalByIdQuery(ObjectId GoalId, ObjectId UserId) : IQuery<DailyGoalResult>;
