using MacroMission.Application.Common.Messaging;
using MacroMission.Application.Meals.Results;
using MongoDB.Bson;

namespace MacroMission.Application.Meals.Queries.GetDailySummary;

public sealed record GetDailySummaryQuery(
    ObjectId UserId,
    DateTime Date) : IQuery<DailySummaryResult>;
