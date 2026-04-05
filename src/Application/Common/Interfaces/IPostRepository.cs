using MacroMission.Domain.Social;
using MongoDB.Bson;

namespace MacroMission.Application.Common.Interfaces;

public interface IPostRepository
{
    Task<Post?> GetByIdAsync(ObjectId id, CancellationToken cancellationToken = default);
    Task<List<Post>> GetFeedAsync(List<ObjectId> authorIds, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<List<Post>> GetByAuthorAsync(ObjectId authorId, ObjectId? requesterId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task CreateAsync(Post post, CancellationToken cancellationToken = default);
    Task UpdateAsync(Post post, CancellationToken cancellationToken = default);
    Task DeleteAsync(ObjectId id, CancellationToken cancellationToken = default);
    Task IncrementLikesAsync(ObjectId id, int delta, CancellationToken cancellationToken = default);
    Task IncrementCommentsAsync(ObjectId id, int delta, CancellationToken cancellationToken = default);
}
