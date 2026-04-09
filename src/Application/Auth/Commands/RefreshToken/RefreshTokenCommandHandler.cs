using MacroMission.Application.Auth.Results;
using MacroMission.Application.Common.Interfaces;
using MacroMission.Application.Common.Messaging;
using MacroMission.Domain.Common;
using MacroMission.Domain.Users;

namespace MacroMission.Application.Auth.Commands.RefreshToken;

internal sealed class RefreshTokenCommandHandler(
    IUserRepository userRepository,
    ITokenService tokenService) : ICommandHandler<RefreshTokenCommand, AuthResult>
{
    public async Task<Result<AuthResult>> Handle(
        RefreshTokenCommand command,
        CancellationToken cancellationToken)
    {
        string hashedToken = tokenService.HashRefreshToken(command.RefreshToken);

        User? user = await userRepository.GetByRefreshTokenAsync(hashedToken, cancellationToken);
        if (user is null)
            return Result<AuthResult>.Failure(Error.Unauthorized("Auth.InvalidToken", "Refresh token is invalid."));

        Domain.Users.RefreshToken? existingToken = user.RefreshTokens
            .FirstOrDefault(t => t.Token == hashedToken);

        if (existingToken is null)
            return Result<AuthResult>.Failure(Error.Unauthorized("Auth.InvalidToken", "Refresh token is invalid."));

        // Reuse detection: token exists in the DB but was already rotated.
        // This means a previously-issued token is being replayed — likely theft.
        // Revoke every active session to force re-login on all devices.
        if (existingToken.RevokedAt is not null)
        {
            user.RevokeAllRefreshTokens();
            await userRepository.UpdateAsync(user, cancellationToken);
            return Result<AuthResult>.Failure(
                Error.Unauthorized("Auth.TokenReuse", "Suspicious activity detected. All sessions have been revoked."));
        }

        if (existingToken.IsExpired)
            return Result<AuthResult>.Failure(Error.Unauthorized("Auth.TokenExpired", "Refresh token has expired. Please log in again."));

        // Rotate: revoke the used token and issue a new one.
        existingToken.RevokedAt = DateTime.UtcNow;

        string newRawToken = tokenService.GenerateRefreshToken();
        string newHashedToken = tokenService.HashRefreshToken(newRawToken);

        user.RemoveExpiredTokens();
        user.RefreshTokens.Add(new Domain.Users.RefreshToken
        {
            Token = newHashedToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        });
        user.UpdatedAt = DateTime.UtcNow;

        await userRepository.UpdateAsync(user, cancellationToken);

        string accessToken = tokenService.GenerateAccessToken(user);
        DateTime accessTokenExpiresAt = DateTime.UtcNow.AddMinutes(15);

        return Result<AuthResult>.Success(new AuthResult(accessToken, newRawToken, accessTokenExpiresAt));
    }
}
