using MacroMission.Application.Common.Interfaces;
using MacroMission.Domain.Social;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MacroMission.Infrastructure.Persistence.Repositories;

public sealed class LikeRepository(IMongoDbContext context) : ILikeRepository
{
    private readonly IMongoCollection<Like> _likes =
        context.GetCollection<Like>("likes");

    public async Task<Like?> GetAsync(ObjectId userId, ObjectId postId, CancellationToken cancellationToken = default)
    {
        FilterDefinition<Like> filter = Builders<Like>.Filter.And(
            Builders<Like>.Filter.Eq(l => l.UserId, userId),
            Builders<Like>.Filter.Eq(l => l.PostId, postId));
        return await _likes.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task CreateAsync(Like like, CancellationToken cancellationToken = default)
    {
        await _likes.InsertOneAsync(like, cancellationToken: cancellationToken);
    }

    public async Task DeleteAsync(ObjectId userId, ObjectId postId, CancellationToken cancellationToken = default)
    {
        FilterDefinition<Like> filter = Builders<Like>.Filter.And(
            Builders<Like>.Filter.Eq(l => l.UserId, userId),
            Builders<Like>.Filter.Eq(l => l.PostId, postId));
        await _likes.DeleteOneAsync(filter, cancellationToken);
    }
}
