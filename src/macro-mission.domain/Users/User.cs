using MacroMission.Domain.Common;

namespace MacroMission.Domain.Users;

public sealed class User : Entity
{
    // Normalized to lowercase before storage — unique index on this field.
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public bool IsEmailVerified { get; set; }
    public string? EmailVerificationToken { get; set; }
    public DateTime? EmailVerificationTokenExpiresAt { get; set; }

    // Embedded to avoid a separate tokens collection on every auth check.
    public List<RefreshToken> RefreshTokens { get; init; } = [];

    public void RevokeRefreshToken(string tokenHash)
    {
        RefreshToken? token = RefreshTokens.FirstOrDefault(t => t.Token == tokenHash);
        if (token is not null)
            token.RevokedAt = DateTime.UtcNow;
    }

    public void RemoveExpiredTokens()
    {
        // Prune stale tokens periodically to keep the document size in check.
        RefreshTokens.RemoveAll(t => !t.IsActive);
    }
}
