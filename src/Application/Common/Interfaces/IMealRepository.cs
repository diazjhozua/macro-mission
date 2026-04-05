using MacroMission.Domain.Meals;
using MongoDB.Bson;

namespace MacroMission.Application.Common.Interfaces;

public interface IMealRepository
{
    Task<Meal?> GetByIdAsync(ObjectId id, CancellationToken cancellationToken = default);
    Task<List<Meal>> GetByDateAsync(ObjectId userId, DateTime date, CancellationToken cancellationToken = default);
    Task CreateAsync(Meal meal, CancellationToken cancellationToken = default);
    Task UpdateAsync(Meal meal, CancellationToken cancellationToken = default);
    Task DeleteAsync(ObjectId id, CancellationToken cancellationToken = default);
}
