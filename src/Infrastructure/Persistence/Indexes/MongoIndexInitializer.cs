using MacroMission.Application.Common.Interfaces;
using MacroMission.Domain.DailyGoals;
using MacroMission.Domain.Foods;
using MacroMission.Domain.Meals;
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
        await CreateDailyGoalIndexesAsync(context);
        await CreateFoodIndexesAsync(context);
        await CreateMealIndexesAsync(context);
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

    private static async Task CreateDailyGoalIndexesAsync(IMongoDbContext context)
    {
        IMongoCollection<DailyGoal> goals = context.GetCollection<DailyGoal>("dailyGoals");

        // Most queries filter by userId — covers GetAll and GetActive lookups.
        IndexKeysDefinition<DailyGoal> userIdIndex = Builders<DailyGoal>.IndexKeys
            .Ascending(g => g.UserId);
        CreateIndexModel<DailyGoal> userId = new(
            userIdIndex,
            new CreateIndexOptions { Name = "dailyGoals_userId" });

        // Compound index for the active goal lookup — hits on every auth-gated request.
        IndexKeysDefinition<DailyGoal> userIdIsActiveIndex = Builders<DailyGoal>.IndexKeys
            .Ascending(g => g.UserId)
            .Ascending(g => g.IsActive);
        CreateIndexModel<DailyGoal> userIdIsActive = new(
            userIdIsActiveIndex,
            new CreateIndexOptions { Name = "dailyGoals_userId_isActive" });

        await goals.Indexes.CreateManyAsync([userId, userIdIsActive]);
    }

    private static async Task CreateFoodIndexesAsync(IMongoDbContext context)
    {
        IMongoCollection<Food> foods = context.GetCollection<Food>("foods");

        // Compound index covers the search query — filters by ownerId then sorts by name.
        IndexKeysDefinition<Food> ownerNameIndex = Builders<Food>.IndexKeys
            .Ascending(f => f.OwnerId)
            .Ascending(f => f.Name);
        CreateIndexModel<Food> ownerName = new(
            ownerNameIndex,
            new CreateIndexOptions { Name = "foods_ownerId_name" });

        await foods.Indexes.CreateManyAsync([ownerName]);
    }

    private static async Task CreateMealIndexesAsync(IMongoDbContext context)
    {
        IMongoCollection<Meal> meals = context.GetCollection<Meal>("meals");

        // Primary query pattern: get all meals for a user on a specific date.
        IndexKeysDefinition<Meal> userDateIndex = Builders<Meal>.IndexKeys
            .Ascending(m => m.UserId)
            .Ascending(m => m.Date);
        CreateIndexModel<Meal> userDate = new(
            userDateIndex,
            new CreateIndexOptions { Name = "meals_userId_date" });

        await meals.Indexes.CreateManyAsync([userDate]);
    }
}
