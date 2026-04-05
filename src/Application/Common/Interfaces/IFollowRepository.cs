using MacroMission.Domain.Social;
using MongoDB.Bson;

namespace MacroMission.Application.Common.Interfaces;

public interface IFollowRepository
{
    Task<Follow?> GetAsync(ObjectId followerId, ObjectId followingId, CancellationToken cancellationToken = default);
    Task<List<ObjectId>> GetFollowingIdsAsync(ObjectId followerId, CancellationToken cancellationToken = default);
    Task<bool> IsFollowingAsync(ObjectId followerId, ObjectId followingId, CancellationToken cancellationToken = default);
    Task CreateAsync(Follow follow, CancellationToken cancellationToken = default);
    Task DeleteAsync(ObjectId followerId, ObjectId followingId, CancellationToken cancellationToken = default);
}
