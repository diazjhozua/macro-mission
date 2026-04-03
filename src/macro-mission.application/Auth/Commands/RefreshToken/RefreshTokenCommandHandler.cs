using ErrorOr;
using MacroMission.Application.Auth.Results;
using MacroMission.Application.Common.Interfaces;
using MacroMission.Domain.Users;
using MediatR;

namespace MacroMission.Application.Auth.Commands.RefreshToken;

public sealed class RefreshTokenCommandHandler(
    IUserRepository userRepository,
    ITokenService tokenService) : IRequestHandler<RefreshTokenCommand, ErrorOr<AuthResult>>
{
    public async Task<ErrorOr<AuthResult>> Handle(
        RefreshTokenCommand command,
        CancellationToken cancellationToken)
    {
        string hashedToken = tokenService.HashRefreshToken(command.RefreshToken);

        // We store hashed tokens — look up by hash so raw tokens never hit the DB.
        User? user = await userRepository.GetByRefreshTokenAsync(hashedToken, cancellationToken);

        if (user is null)
            return Error.Unauthorized("Auth.InvalidToken", "Refresh token is invalid.");

        Domain.Users.RefreshToken? existingToken = user.RefreshTokens
            .FirstOrDefault(t => t.Token == hashedToken);

        if (existingToken is null || !existingToken.IsActive)
            return Error.Unauthorized("Auth.InvalidToken", "Refresh token is expired or revoked.");

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

        return new AuthResult(accessToken, newRawToken, accessTokenExpiresAt);
    }
}
