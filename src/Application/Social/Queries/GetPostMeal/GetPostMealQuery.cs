using MacroMission.Application.Common.Messaging;
using MacroMission.Application.Meals.Results;
using MongoDB.Bson;

namespace MacroMission.Application.Social.Queries.GetPostMeal;

public sealed record GetPostMealQuery(ObjectId PostId, ObjectId RequesterId) : IQuery<MealResult>;
