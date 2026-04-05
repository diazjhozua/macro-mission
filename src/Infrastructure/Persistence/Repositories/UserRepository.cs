using MacroMission.Application.Common.Interfaces;
using MacroMission.Domain.Users;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MacroMission.Infrastructure.Persistence.Repositories;

public sealed class UserRepository(IMongoDbContext context) : IUserRepository
{
    private readonly IMongoCollection<User> _users =
        context.GetCollection<User>("users");

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        FilterDefinition<User> filter = Builders<User>.Filter.Eq(u => u.Email, email);
        return await _users.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<User?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        FilterDefinition<User> filter = Builders<User>.Filter.Eq(u => u.Id, ObjectId.Parse(id));
        return await _users.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<User?> GetByEmailVerificationTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        FilterDefinition<User> filter = Builders<User>.Filter.Eq(u => u.EmailVerificationToken, token);
        return await _users.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<User?> GetByRefreshTokenAsync(string hashedToken, CancellationToken cancellationToken = default)
    {
        // Query into the embedded refreshTokens array.
        FilterDefinition<User> filter = Builders<User>.Filter
            .ElemMatch(u => u.RefreshTokens, t => t.Token == hashedToken);

        return await _users.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        FilterDefinition<User> filter = Builders<User>.Filter.Eq(u => u.Email, email);
        return await _users.Find(filter).AnyAsync(cancellationToken);
    }

    public async Task CreateAsync(User user, CancellationToken cancellationToken = default)
    {
        await _users.InsertOneAsync(user, cancellationToken: cancellationToken);
    }

    public async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        FilterDefinition<User> filter = Builders<User>.Filter.Eq(u => u.Id, user.Id);
        await _users.ReplaceOneAsync(filter, user, cancellationToken: cancellationToken);
    }
}
