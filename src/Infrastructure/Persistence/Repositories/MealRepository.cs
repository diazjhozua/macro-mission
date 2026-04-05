using MacroMission.Application.Common.Interfaces;
using MacroMission.Domain.Meals;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MacroMission.Infrastructure.Persistence.Repositories;

public sealed class MealRepository(IMongoDbContext context) : IMealRepository
{
    private readonly IMongoCollection<Meal> _meals =
        context.GetCollection<Meal>("meals");

    public async Task<Meal?> GetByIdAsync(ObjectId id, CancellationToken cancellationToken = default)
    {
        FilterDefinition<Meal> filter = Builders<Meal>.Filter.Eq(m => m.Id, id);
        return await _meals.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<Meal>> GetByDateAsync(
        ObjectId userId,
        DateTime date,
        CancellationToken cancellationToken = default)
    {
        FilterDefinition<Meal> filter = Builders<Meal>.Filter.And(
            Builders<Meal>.Filter.Eq(m => m.UserId, userId),
            Builders<Meal>.Filter.Eq(m => m.Date, date));

        return await _meals.Find(filter)
            .SortBy(m => m.LoggedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task CreateAsync(Meal meal, CancellationToken cancellationToken = default)
    {
        await _meals.InsertOneAsync(meal, cancellationToken: cancellationToken);
    }

    public async Task DeleteAsync(ObjectId id, CancellationToken cancellationToken = default)
    {
        FilterDefinition<Meal> filter = Builders<Meal>.Filter.Eq(m => m.Id, id);
        await _meals.DeleteOneAsync(filter, cancellationToken);
    }
}
