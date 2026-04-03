namespace MacroMission.Domain.Users;

/// <summary>Embedded in the User document — avoids a separate collection for token lookups.</summary>
public sealed class RefreshToken
{
    public string Token { get; init; } = string.Empty;  // stored as a hash
    public DateTime ExpiresAt { get; init; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime? RevokedAt { get; set; }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsActive => RevokedAt is null && !IsExpired;
}
