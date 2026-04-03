using MacroMission.Application.Common.Interfaces;
using MacroMission.Domain.Users;
using MongoDB.Driver;

namespace MacroMission.Infrastructure.Persistence.Indexes;

/// <summary>
/// Creates all indexes at startup. CreateIndexAsync is idempotent — safe to run on every boot.
/// Indexes are defined here rather than in migrations so there's one place to audit them.
/// </summary>
public static class MongoIndexInitializer
{
    public static async Task InitializeAsync(IMongoDbContext context)
    {
        await CreateUserIndexesAsync(context);
    }

    private static async Task CreateUserIndexesAsync(IMongoDbContext context)
    {
        IMongoCollection<User> users = context.GetCollection<User>("users");

        // Unique index on normalized email — prevents race-condition duplicates at the DB level.
        IndexKeysDefinition<User> emailIndex = Builders<User>.IndexKeys.Ascending(u => u.Email);
        CreateIndexModel<User> uniqueEmail = new(
            emailIndex,
            new CreateIndexOptions { Unique = true, Name = "email_unique" });

        // Needed for refresh token lookup — embedded array field.
        IndexKeysDefinition<User> refreshTokenIndex = Builders<User>.IndexKeys
            .Ascending("refreshTokens.token");
        CreateIndexModel<User> refreshToken = new(
            refreshTokenIndex,
            new CreateIndexOptions { Name = "refreshTokens_token" });

        await users.Indexes.CreateManyAsync([uniqueEmail, refreshToken]);
    }
}
