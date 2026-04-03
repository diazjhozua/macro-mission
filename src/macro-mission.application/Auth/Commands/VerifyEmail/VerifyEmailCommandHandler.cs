using ErrorOr;
using MacroMission.Application.Common.Interfaces;
using MacroMission.Domain.Users;
using MediatR;

namespace MacroMission.Application.Auth.Commands.VerifyEmail;

public sealed class VerifyEmailCommandHandler(
    IUserRepository userRepository) : IRequestHandler<VerifyEmailCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(
        VerifyEmailCommand command,
        CancellationToken cancellationToken)
    {
        User? user = await userRepository.GetByEmailVerificationTokenAsync(
            command.Token, cancellationToken);

        if (user is null)
            return Error.NotFound("Auth.InvalidToken", "Verification token is invalid.");

        if (user.EmailVerificationTokenExpiresAt < DateTime.UtcNow)
            return Error.Validation("Auth.TokenExpired", "Verification token has expired.");

        if (user.IsEmailVerified)
            return Error.Conflict("Auth.AlreadyVerified", "Email is already verified.");

        user.IsEmailVerified = true;
        user.EmailVerificationToken = null;
        user.EmailVerificationTokenExpiresAt = null;
        user.UpdatedAt = DateTime.UtcNow;

        await userRepository.UpdateAsync(user, cancellationToken);

        return Result.Success;
    }
}
