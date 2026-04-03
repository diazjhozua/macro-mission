namespace MacroMission.Infrastructure.Auth;

public sealed class ResendSettings
{
    public const string SectionName = "Resend";

    public string ApiKey { get; init; } = string.Empty;
    public string FromEmail { get; init; } = string.Empty;
    public string AppName { get; init; } = string.Empty;

    // Prepends "[TEST]" to all email subjects when not in production.
    public bool IsProduction { get; init; }
}
