namespace MacroMission.Application.Auth.Results;

/// <summary>Internal result returned from auth handlers — mapped to AuthResponse at the API layer.</summary>
public sealed record AuthResult(
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiresAt);
