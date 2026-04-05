using MacroMission.Application.Common.Interfaces;
using MacroMission.Application.Common.Messaging;
using MacroMission.Domain.Common;
using MacroMission.Domain.Users;

namespace MacroMission.Application.Auth.Commands.Register;

internal sealed class RegisterCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IEmailService emailService) : ICommandHandler<RegisterCommand>
{
    public async Task<Result> Handle(RegisterCommand command, CancellationToken cancellationToken)
    {
        string normalizedEmail = command.Email.ToLowerInvariant();

        bool emailTaken = await userRepository.ExistsByEmailAsync(normalizedEmail, cancellationToken);
        if (emailTaken)
            return Result.Failure(Error.Conflict("Auth.EmailTaken", "An account with this email already exists."));

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
            EmailVerificationTokenExpiresAt = DateTime.UtcNow.AddHours(24)
        };

        await userRepository.CreateAsync(user, cancellationToken);
        await emailService.SendEmailVerificationAsync(normalizedEmail, verificationToken, cancellationToken);

        return Result.Success();
    }
}
