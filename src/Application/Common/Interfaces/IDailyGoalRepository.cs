using MacroMission.Domain.DailyGoals;
using MongoDB.Bson;

namespace MacroMission.Application.Common.Interfaces;

public interface IDailyGoalRepository
{
    Task<List<DailyGoal>> GetAllByUserIdAsync(ObjectId userId, CancellationToken cancellationToken = default);
    Task<DailyGoal?> GetActiveByUserIdAsync(ObjectId userId, CancellationToken cancellationToken = default);
    Task<DailyGoal?> GetByIdAsync(ObjectId id, CancellationToken cancellationToken = default);
    Task CreateAsync(DailyGoal goal, CancellationToken cancellationToken = default);
    Task UpdateAsync(DailyGoal goal, CancellationToken cancellationToken = default);
    Task DeactivateAllByUserIdAsync(ObjectId userId, CancellationToken cancellationToken = default);
    Task DeleteAsync(ObjectId id, CancellationToken cancellationToken = default);
}
