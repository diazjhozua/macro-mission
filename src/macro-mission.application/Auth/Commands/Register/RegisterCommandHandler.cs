using ErrorOr;
using MacroMission.Application.Common.Interfaces;
using MacroMission.Domain.Users;
using MediatR;

namespace MacroMission.Application.Auth.Commands.Register;

public sealed class RegisterCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IEmailService emailService) : IRequestHandler<RegisterCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(
        RegisterCommand command,
        CancellationToken cancellationToken)
    {
        string normalizedEmail = command.Email.ToLowerInvariant();

        bool emailTaken = await userRepository.ExistsByEmailAsync(normalizedEmail, cancellationToken);
        if (emailTaken)
            return Error.Conflict("Auth.EmailTaken", "An account with this email already exists.");

        string verificationToken = Guid.NewGuid().ToString("N");

        User user = new()
        {
            Email = normalizedEmail,
            PasswordHash = passwordHasher.Hash(command.Password),
            FirstName = command.FirstName,
            LastName = command.LastName,
            Nickname = command.Nickname,
            IsEmailVerified = false,
            EmailVerificationToken = verificationToken,
            // Token expires in 24 hours — long enough to be convenient, short enough to limit exposure.
            EmailVerificationTokenExpiresAt = DateTime.UtcNow.AddHours(24)
        };

        await userRepository.CreateAsync(user, cancellationToken);
        await emailService.SendEmailVerificationAsync(normalizedEmail, verificationToken, cancellationToken);

        return Result.Success;
    }
}
