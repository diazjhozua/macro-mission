using MacroMission.Application.Common.Interfaces;
using MacroMission.Domain.DailyGoals;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MacroMission.Infrastructure.Persistence.Repositories;

public sealed class DailyGoalRepository(IMongoDbContext context) : IDailyGoalRepository
{
    private readonly IMongoCollection<DailyGoal> _goals =
        context.GetCollection<DailyGoal>("dailyGoals");

    public async Task<List<DailyGoal>> GetAllByUserIdAsync(ObjectId userId, CancellationToken cancellationToken = default)
    {
        FilterDefinition<DailyGoal> filter = Builders<DailyGoal>.Filter.Eq(g => g.UserId, userId);
        return await _goals.Find(filter).ToListAsync(cancellationToken);
    }

    public async Task<DailyGoal?> GetActiveByUserIdAsync(ObjectId userId, CancellationToken cancellationToken = default)
    {
        FilterDefinition<DailyGoal> filter = Builders<DailyGoal>.Filter.And(
            Builders<DailyGoal>.Filter.Eq(g => g.UserId, userId),
            Builders<DailyGoal>.Filter.Eq(g => g.IsActive, true));

        return await _goals.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<DailyGoal?> GetByIdAsync(ObjectId id, CancellationToken cancellationToken = default)
    {
        FilterDefinition<DailyGoal> filter = Builders<DailyGoal>.Filter.Eq(g => g.Id, id);
        return await _goals.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task CreateAsync(DailyGoal goal, CancellationToken cancellationToken = default)
    {
        await _goals.InsertOneAsync(goal, cancellationToken: cancellationToken);
    }

    public async Task UpdateAsync(DailyGoal goal, CancellationToken cancellationToken = default)
    {
        FilterDefinition<DailyGoal> filter = Builders<DailyGoal>.Filter.Eq(g => g.Id, goal.Id);
        await _goals.ReplaceOneAsync(filter, goal, cancellationToken: cancellationToken);
    }

    public async Task DeactivateAllByUserIdAsync(ObjectId userId, CancellationToken cancellationToken = default)
    {
        FilterDefinition<DailyGoal> filter = Builders<DailyGoal>.Filter.Eq(g => g.UserId, userId);
        UpdateDefinition<DailyGoal> update = Builders<DailyGoal>.Update
            .Set(g => g.IsActive, false)
            .Set(g => g.UpdatedAt, DateTime.UtcNow);

        await _goals.UpdateManyAsync(filter, update, cancellationToken: cancellationToken);
    }
}
