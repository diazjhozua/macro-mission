using MacroMission.Application.Common.Interfaces;
using MacroMission.Application.Common.Messaging;
using MacroMission.Domain.Common;
using MacroMission.Domain.Users;

namespace MacroMission.Application.Auth.Commands.VerifyEmail;

internal sealed class VerifyEmailCommandHandler(
    IUserRepository userRepository) : ICommandHandler<VerifyEmailCommand>
{
    public async Task<Result> Handle(VerifyEmailCommand command, CancellationToken cancellationToken)
    {
        User? user = await userRepository.GetByEmailVerificationTokenAsync(
            command.Token, cancellationToken);

        if (user is null)
            return Result.Failure(Error.NotFound("Auth.InvalidToken", "Verification token is invalid."));

        if (user.EmailVerificationTokenExpiresAt < DateTime.UtcNow)
            return Result.Failure(Error.Validation("Auth.TokenExpired", "Verification token has expired."));

        if (user.IsEmailVerified)
            return Result.Failure(Error.Conflict("Auth.AlreadyVerified", "Email is already verified."));

        user.IsEmailVerified = true;
        user.EmailVerificationToken = null;
        user.EmailVerificationTokenExpiresAt = null;
        user.UpdatedAt = DateTime.UtcNow;

        await userRepository.UpdateAsync(user, cancellationToken);

        return Result.Success();
    }
}
