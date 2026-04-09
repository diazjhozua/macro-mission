using MacroMission.Domain.Common;

namespace MacroMission.Domain.Users;

public sealed class User : Entity
{
    // Normalized to lowercase before storage — unique index on this field.
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Nickname { get; set; } = string.Empty;
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

    /// <summary>
    /// Revokes every active token. Called when token reuse is detected to force
    /// re-login on all devices.
    /// </summary>
    public void RevokeAllRefreshTokens()
    {
        foreach (RefreshToken token in RefreshTokens.Where(t => t.IsActive))
            token.RevokedAt = DateTime.UtcNow;
    }

    public void RemoveExpiredTokens()
    {
        // Only prune tokens past their expiry date. Revoked-but-not-yet-expired
        // tokens are kept so replay attacks can be detected.
        RefreshTokens.RemoveAll(t => t.IsExpired);
    }
}
