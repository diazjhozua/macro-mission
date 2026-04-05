using MacroMission.Application.Common.Interfaces;
using MacroMission.Domain.Social;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MacroMission.Infrastructure.Persistence.Repositories;

public sealed class FollowRepository(IMongoDbContext context) : IFollowRepository
{
    private readonly IMongoCollection<Follow> _follows =
        context.GetCollection<Follow>("follows");

    public async Task<Follow?> GetAsync(ObjectId followerId, ObjectId followingId, CancellationToken cancellationToken = default)
    {
        FilterDefinition<Follow> filter = Builders<Follow>.Filter.And(
            Builders<Follow>.Filter.Eq(f => f.FollowerId, followerId),
            Builders<Follow>.Filter.Eq(f => f.FollowingId, followingId));
        return await _follows.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<ObjectId>> GetFollowingIdsAsync(ObjectId followerId, CancellationToken cancellationToken = default)
    {
        FilterDefinition<Follow> filter = Builders<Follow>.Filter.Eq(f => f.FollowerId, followerId);
        List<Follow> follows = await _follows.Find(filter).ToListAsync(cancellationToken);
        return follows.Select(f => f.FollowingId).ToList();
    }

    public async Task<List<ObjectId>> GetFollowerIdsAsync(ObjectId followingId, CancellationToken cancellationToken = default)
    {
        FilterDefinition<Follow> filter = Builders<Follow>.Filter.Eq(f => f.FollowingId, followingId);
        List<Follow> follows = await _follows.Find(filter).ToListAsync(cancellationToken);
        return follows.Select(f => f.FollowerId).ToList();
    }

    public async Task<bool> IsFollowingAsync(ObjectId followerId, ObjectId followingId, CancellationToken cancellationToken = default)
    {
        FilterDefinition<Follow> filter = Builders<Follow>.Filter.And(
            Builders<Follow>.Filter.Eq(f => f.FollowerId, followerId),
            Builders<Follow>.Filter.Eq(f => f.FollowingId, followingId));
        return await _follows.Find(filter).AnyAsync(cancellationToken);
    }

    public async Task CreateAsync(Follow follow, CancellationToken cancellationToken = default)
    {
        await _follows.InsertOneAsync(follow, cancellationToken: cancellationToken);
    }

    public async Task DeleteAsync(ObjectId followerId, ObjectId followingId, CancellationToken cancellationToken = default)
    {
        FilterDefinition<Follow> filter = Builders<Follow>.Filter.And(
            Builders<Follow>.Filter.Eq(f => f.FollowerId, followerId),
            Builders<Follow>.Filter.Eq(f => f.FollowingId, followingId));
        await _follows.DeleteOneAsync(filter, cancellationToken);
    }
}
