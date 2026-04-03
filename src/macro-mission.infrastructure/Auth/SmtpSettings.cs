namespace MacroMission.Infrastructure.Auth;

public sealed class SmtpSettings
{
    public const string SectionName = "Smtp";

    public string Host { get; init; } = string.Empty;
    public int Port { get; init; } = 587;
    public string Username { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string FromEmail { get; init; } = string.Empty;
    public string AppName { get; init; } = string.Empty;

    // Prepends "[TEST]" to all email subjects when not in production.
    public bool IsProduction { get; init; }
}
