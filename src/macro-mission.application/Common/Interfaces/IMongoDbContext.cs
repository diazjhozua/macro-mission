using MongoDB.Driver;

namespace MacroMission.Application.Common.Interfaces;

/// <summary>Thin abstraction so the application layer never imports MongoDB.Driver directly.</summary>
public interface IMongoDbContext
{
    IMongoCollection<T> GetCollection<T>(string name);
}
