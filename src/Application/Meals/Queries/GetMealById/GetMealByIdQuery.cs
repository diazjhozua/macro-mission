using MacroMission.Application.Common.Messaging;
using MacroMission.Application.Meals.Results;
using MongoDB.Bson;

namespace MacroMission.Application.Meals.Queries.GetMealById;

public sealed record GetMealByIdQuery(ObjectId MealId, ObjectId UserId) : IQuery<MealResult>;
