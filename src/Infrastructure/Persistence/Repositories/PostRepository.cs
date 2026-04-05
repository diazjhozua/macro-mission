using MacroMission.Application.Common.Interfaces;
using MacroMission.Domain.Social;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MacroMission.Infrastructure.Persistence.Repositories;

public sealed class PostRepository(IMongoDbContext context) : IPostRepository
{
    private readonly IMongoCollection<Post> _posts =
        context.GetCollection<Post>("posts");

    public async Task<Post?> GetByIdAsync(ObjectId id, CancellationToken cancellationToken = default)
    {
        FilterDefinition<Post> filter = Builders<Post>.Filter.Eq(p => p.Id, id);
        return await _posts.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<Post>> GetPublicPostsAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        FilterDefinition<Post> filter = Builders<Post>.Filter.Eq(p => p.Visibility, PostVisibility.Public);

        return await _posts.Find(filter)
            .SortByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Post>> GetFeedAsync(List<ObjectId> authorIds, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        // Feed shows public + followersOnly posts from followed users, sorted newest first.
        FilterDefinition<Post> filter = Builders<Post>.Filter.And(
            Builders<Post>.Filter.In(p => p.AuthorId, authorIds),
            Builders<Post>.Filter.Ne(p => p.Visibility, PostVisibility.Private));

        return await _posts.Find(filter)
            .SortByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Post>> GetByAuthorAsync(ObjectId authorId, ObjectId? requesterId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        // When viewing another user's profile, hide private posts.
        FilterDefinition<Post> filter = requesterId.HasValue
            ? Builders<Post>.Filter.And(
                Builders<Post>.Filter.Eq(p => p.AuthorId, authorId),
                Builders<Post>.Filter.Ne(p => p.Visibility, PostVisibility.Private))
            : Builders<Post>.Filter.Eq(p => p.AuthorId, authorId);

        return await _posts.Find(filter)
            .SortByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task CreateAsync(Post post, CancellationToken cancellationToken = default)
    {
        await _posts.InsertOneAsync(post, cancellationToken: cancellationToken);
    }

    public async Task UpdateAsync(Post post, CancellationToken cancellationToken = default)
    {
        FilterDefinition<Post> filter = Builders<Post>.Filter.Eq(p => p.Id, post.Id);
        await _posts.ReplaceOneAsync(filter, post, cancellationToken: cancellationToken);
    }

    public async Task DeleteAsync(ObjectId id, CancellationToken cancellationToken = default)
    {
        FilterDefinition<Post> filter = Builders<Post>.Filter.Eq(p => p.Id, id);
        await _posts.DeleteOneAsync(filter, cancellationToken);
    }

    public async Task IncrementLikesAsync(ObjectId id, int delta, CancellationToken cancellationToken = default)
    {
        FilterDefinition<Post> filter = Builders<Post>.Filter.Eq(p => p.Id, id);
        UpdateDefinition<Post> update = Builders<Post>.Update.Inc(p => p.LikesCount, delta);
        await _posts.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
    }

    public async Task IncrementCommentsAsync(ObjectId id, int delta, CancellationToken cancellationToken = default)
    {
        FilterDefinition<Post> filter = Builders<Post>.Filter.Eq(p => p.Id, id);
        UpdateDefinition<Post> update = Builders<Post>.Update.Inc(p => p.CommentsCount, delta);
        await _posts.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
    }
}
