using MacroMission.Domain.Foods;
using MongoDB.Bson;

namespace MacroMission.Application.Common.Interfaces;

public interface IFoodRepository
{
    Task<Food?> GetByIdAsync(ObjectId id, CancellationToken cancellationToken = default);
    Task<List<Food>> SearchAsync(string term, ObjectId? userId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task CreateAsync(Food food, CancellationToken cancellationToken = default);
    Task UpdateAsync(Food food, CancellationToken cancellationToken = default);
    Task DeleteAsync(ObjectId id, CancellationToken cancellationToken = default);
}
