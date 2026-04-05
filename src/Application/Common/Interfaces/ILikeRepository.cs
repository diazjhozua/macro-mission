using MacroMission.Domain.Social;
using MongoDB.Bson;

namespace MacroMission.Application.Common.Interfaces;

public interface ILikeRepository
{
    Task<Like?> GetAsync(ObjectId userId, ObjectId postId, CancellationToken cancellationToken = default);
    Task CreateAsync(Like like, CancellationToken cancellationToken = default);
    Task DeleteAsync(ObjectId userId, ObjectId postId, CancellationToken cancellationToken = default);
}
