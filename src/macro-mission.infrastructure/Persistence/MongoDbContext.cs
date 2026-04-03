using MacroMission.Application.Common.Interfaces;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace MacroMission.Infrastructure.Persistence;

public sealed class MongoDbContext : IMongoDbContext
{
    private readonly IMongoDatabase _database;

    static MongoDbContext()
    {
        // Apply camelCase + ignore-extra-elements globally once at startup.
        // IgnoreExtraElements means adding new fields to documents won't break existing reads.
        ConventionPack pack = new()
        {
            new CamelCaseElementNameConvention(),
            new IgnoreExtraElementsConvention(true)
        };
        ConventionRegistry.Register("MacroMissionDefaults", pack, _ => true);
    }

    public MongoDbContext(MongoDbSettings settings)
    {
        MongoClient client = new(settings.ConnectionString);
        _database = client.GetDatabase(settings.DatabaseName);
    }

    public IMongoCollection<T> GetCollection<T>(string name) =>
        _database.GetCollection<T>(name);
}
