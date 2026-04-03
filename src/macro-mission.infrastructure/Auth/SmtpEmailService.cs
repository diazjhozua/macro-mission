using MacroMission.Application.Common.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace MacroMission.Infrastructure.Auth;

public sealed class SmtpEmailService(SmtpSettings settings) : IEmailService
{
    public async Task SendEmailVerificationAsync(
        string toEmail,
        string verificationToken,
        CancellationToken cancellationToken = default)
    {
        MimeMessage message = BuildMessage(
            toEmail,
            BuildSubject("Verify your email address"),
            BuildVerificationBody(verificationToken));

        await SendAsync(message, cancellationToken);
    }

    private async Task SendAsync(MimeMessage message, CancellationToken cancellationToken)
    {
        using SmtpClient client = new();

        // Port 587 with STARTTLS is the standard for Gmail app passwords.
        await client.ConnectAsync(settings.Host, settings.Port, SecureSocketOptions.StartTls, cancellationToken);
        await client.AuthenticateAsync(settings.Username, settings.Password, cancellationToken);
        await client.SendAsync(message, cancellationToken);
        await client.DisconnectAsync(true, cancellationToken);
    }

    private MimeMessage BuildMessage(string toEmail, string subject, string htmlBody)
    {
        MimeMessage message = new();
        message.From.Add(MailboxAddress.Parse(settings.FromEmail));
        message.To.Add(MailboxAddress.Parse(toEmail));
        message.Subject = subject;
        message.Body = new TextPart("html") { Text = htmlBody };
        return message;
    }

    private string BuildSubject(string subject)
    {
        string appPrefix = $"[{settings.AppName}]";
        string testPrefix = settings.IsProduction ? string.Empty : "[TEST] ";
        return $"{testPrefix}{appPrefix} {subject}";
    }

    private static string BuildVerificationBody(string token) =>
        $"""
        <h2>Welcome to MacroMission!</h2>
        <p>Use the token below to verify your email address:</p>
        <p><strong>{token}</strong></p>
        <p>This token expires in 24 hours.</p>
        """;
}
