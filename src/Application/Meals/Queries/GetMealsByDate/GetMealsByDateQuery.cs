using MacroMission.Application.Common.Messaging;
using MacroMission.Application.Meals.Results;
using MongoDB.Bson;

namespace MacroMission.Application.Meals.Queries.GetMealsByDate;

public sealed record GetMealsByDateQuery(
    ObjectId UserId,
    DateTime Date) : IQuery<List<MealResult>>;
