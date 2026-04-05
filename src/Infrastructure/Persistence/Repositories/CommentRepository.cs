using MacroMission.Application.Common.Interfaces;
using MacroMission.Domain.Social;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MacroMission.Infrastructure.Persistence.Repositories;

public sealed class CommentRepository(IMongoDbContext context) : ICommentRepository
{
    private readonly IMongoCollection<Comment> _comments =
        context.GetCollection<Comment>("comments");

    public async Task<Comment?> GetByIdAsync(ObjectId id, CancellationToken cancellationToken = default)
    {
        FilterDefinition<Comment> filter = Builders<Comment>.Filter.Eq(c => c.Id, id);
        return await _comments.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<Comment>> GetByPostAsync(ObjectId postId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        FilterDefinition<Comment> filter = Builders<Comment>.Filter.Eq(c => c.PostId, postId);
        return await _comments.Find(filter)
            .SortBy(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task CreateAsync(Comment comment, CancellationToken cancellationToken = default)
    {
        await _comments.InsertOneAsync(comment, cancellationToken: cancellationToken);
    }

    public async Task DeleteAsync(ObjectId id, CancellationToken cancellationToken = default)
    {
        FilterDefinition<Comment> filter = Builders<Comment>.Filter.Eq(c => c.Id, id);
        await _comments.DeleteOneAsync(filter, cancellationToken);
    }
}
