using MacroMission.Application.Common.Interfaces;
using Resend;

namespace MacroMission.Infrastructure.Auth;

public sealed class ResendEmailService(IResend resend, ResendSettings settings) : IEmailService
{
    public async Task SendEmailVerificationAsync(
        string toEmail,
        string verificationToken,
        CancellationToken cancellationToken = default)
    {
        EmailMessage message = new()
        {
            From = settings.FromEmail,
            To = { toEmail },
            Subject = BuildSubject("Verify your MacroMission account"),
            HtmlBody = BuildVerificationEmail(verificationToken)
        };

        await resend.EmailSendAsync(message, cancellationToken);
    }

    private string BuildSubject(string subject)
    {
        string appPrefix = $"[{settings.AppName}]";
        string testPrefix = settings.IsProduction ? string.Empty : "[TEST] ";
        return $"{testPrefix}{appPrefix} {subject}";
    }

    private static string BuildVerificationEmail(string token) =>
        $"""
        <h2>Welcome to MacroMission!</h2>
        <p>Use the token below to verify your email address:</p>
        <p><strong>{token}</strong></p>
        <p>This token expires in 24 hours.</p>
        """;
}
