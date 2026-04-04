using MacroMission.Application.Common.Messaging;
using MacroMission.Application.Foods.Results;
using MongoDB.Bson;

namespace MacroMission.Application.Foods.Queries.SearchFoods;

public sealed record SearchFoodsQuery(
    string Term,
    ObjectId UserId,
    int Page,
    int PageSize) : IQuery<List<FoodResult>>;
