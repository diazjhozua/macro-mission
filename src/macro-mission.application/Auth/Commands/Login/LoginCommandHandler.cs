using ErrorOr;
using MacroMission.Application.Auth.Results;
using MacroMission.Application.Common.Interfaces;
using MacroMission.Domain.Users;
using MediatR;
using DomainRefreshToken = MacroMission.Domain.Users.RefreshToken;

namespace MacroMission.Application.Auth.Commands.Login;

public sealed class LoginCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    ITokenService tokenService) : IRequestHandler<LoginCommand, ErrorOr<AuthResult>>
{
    public async Task<ErrorOr<AuthResult>> Handle(
        LoginCommand command,
        CancellationToken cancellationToken)
    {
        User? user = await userRepository.GetByEmailAsync(
            command.Email.ToLowerInvariant(), cancellationToken);

        // Use a generic message to avoid leaking whether the email exists.
        if (user is null || !passwordHasher.Verify(command.Password, user.PasswordHash))
            return Error.Unauthorized("Auth.InvalidCredentials", "Invalid email or password.");

        if (!user.IsEmailVerified)
            return Error.Forbidden("Auth.EmailNotVerified", "Please verify your email before logging in.");

        string rawRefreshToken = tokenService.GenerateRefreshToken();
        string hashedRefreshToken = tokenService.HashRefreshToken(rawRefreshToken);

        user.RemoveExpiredTokens();
        user.RefreshTokens.Add(new DomainRefreshToken
        {
            Token = hashedRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        });
        user.UpdatedAt = DateTime.UtcNow;

        await userRepository.UpdateAsync(user, cancellationToken);

        string accessToken = tokenService.GenerateAccessToken(user);
        DateTime accessTokenExpiresAt = DateTime.UtcNow.AddMinutes(15);

        return new AuthResult(accessToken, rawRefreshToken, accessTokenExpiresAt);
    }
}
