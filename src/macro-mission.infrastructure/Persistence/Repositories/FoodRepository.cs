using MacroMission.Application.Common.Interfaces;
using MacroMission.Domain.Foods;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MacroMission.Infrastructure.Persistence.Repositories;

public sealed class FoodRepository(IMongoDbContext context) : IFoodRepository
{
    private readonly IMongoCollection<Food> _foods =
        context.GetCollection<Food>("foods");

    public async Task<Food?> GetByIdAsync(ObjectId id, CancellationToken cancellationToken = default)
    {
        FilterDefinition<Food> filter = Builders<Food>.Filter.Eq(f => f.Id, id);
        return await _foods.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<Food>> SearchAsync(
        string term,
        ObjectId? userId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        // Returns global foods + the requesting user's custom foods in one query.
        FilterDefinition<Food> ownerFilter = Builders<Food>.Filter.Or(
            Builders<Food>.Filter.Eq(f => f.OwnerId, null),
            Builders<Food>.Filter.Eq(f => f.OwnerId, userId));

        FilterDefinition<Food> searchFilter = string.IsNullOrWhiteSpace(term)
            ? ownerFilter
            : Builders<Food>.Filter.And(
                ownerFilter,
                Builders<Food>.Filter.Regex(f => f.Name,
                    new BsonRegularExpression(term, "i")));

        return await _foods.Find(searchFilter)
            .SortBy(f => f.Name)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task CreateAsync(Food food, CancellationToken cancellationToken = default)
    {
        await _foods.InsertOneAsync(food, cancellationToken: cancellationToken);
    }

    public async Task UpdateAsync(Food food, CancellationToken cancellationToken = default)
    {
        FilterDefinition<Food> filter = Builders<Food>.Filter.Eq(f => f.Id, food.Id);
        await _foods.ReplaceOneAsync(filter, food, cancellationToken: cancellationToken);
    }

    public async Task DeleteAsync(ObjectId id, CancellationToken cancellationToken = default)
    {
        FilterDefinition<Food> filter = Builders<Food>.Filter.Eq(f => f.Id, id);
        await _foods.DeleteOneAsync(filter, cancellationToken);
    }
}
