using MacroMission.Domain.Social;
using MongoDB.Bson;

namespace MacroMission.Application.Common.Interfaces;

public interface ICommentRepository
{
    Task<Comment?> GetByIdAsync(ObjectId id, CancellationToken cancellationToken = default);
    Task<List<Comment>> GetByPostAsync(ObjectId postId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task CreateAsync(Comment comment, CancellationToken cancellationToken = default);
    Task DeleteAsync(ObjectId id, CancellationToken cancellationToken = default);
}
