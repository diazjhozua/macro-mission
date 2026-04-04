using MacroMission.Application.Common.Messaging;
using MacroMission.Application.Foods.Results;
using MongoDB.Bson;

namespace MacroMission.Application.Foods.Queries.GetFoodById;

public sealed record GetFoodByIdQuery(ObjectId FoodId, ObjectId UserId) : IQuery<FoodResult>;
