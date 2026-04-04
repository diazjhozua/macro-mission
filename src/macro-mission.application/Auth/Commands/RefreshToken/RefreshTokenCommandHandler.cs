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

        if (existingToken is null || !existingToken.IsActive)
            return Result<AuthResult>.Failure(Error.Unauthorized("Auth.InvalidToken", "Refresh token is expired or revoked."));

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
