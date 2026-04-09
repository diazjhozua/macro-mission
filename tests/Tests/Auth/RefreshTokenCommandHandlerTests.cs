using FluentAssertions;
using MacroMission.Application.Auth.Commands.RefreshToken;
using MacroMission.Application.Auth.Results;
using MacroMission.Application.Common.Interfaces;
using MacroMission.Domain.Common;
using MacroMission.Domain.Users;
using NSubstitute;

namespace MacroMission.Tests.Auth;

public sealed class RefreshTokenCommandHandlerTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly ITokenService _tokenService = Substitute.For<ITokenService>();
    private readonly RefreshTokenCommandHandler _handler;

    public RefreshTokenCommandHandlerTests()
    {
        _handler = new RefreshTokenCommandHandler(_userRepository, _tokenService);
    }

    [Fact]
    public async Task Handle_WithValidToken_RotatesTokenAndReturnsAuthResult()
    {
        // Arrange
        RefreshToken activeToken = new()
        {
            Token = "hashed-token",
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };
        User user = new() { RefreshTokens = [activeToken] };

        _tokenService.HashRefreshToken("raw-token").Returns("hashed-token");
        _userRepository.GetByRefreshTokenAsync("hashed-token").Returns(user);
        _tokenService.GenerateRefreshToken().Returns("new-raw-token");
        _tokenService.HashRefreshToken("new-raw-token").Returns("new-hashed-token");
        _tokenService.GenerateAccessToken(user).Returns("new-access-token");

        // Act
        Result<AuthResult> result = await _handler.Handle(
            new RefreshTokenCommand("raw-token"), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.AccessToken.Should().Be("new-access-token");
        result.Value.RefreshToken.Should().Be("new-raw-token");

        // Old token must be revoked, new token added.
        activeToken.RevokedAt.Should().NotBeNull();
        user.RefreshTokens.Should().Contain(t => t.Token == "new-hashed-token");
        await _userRepository.Received(1).UpdateAsync(user, CancellationToken.None);
    }

    [Fact]
    public async Task Handle_WithUnknownToken_ReturnsUnauthorized()
    {
        // Arrange
        _tokenService.HashRefreshToken("unknown-token").Returns("hashed-unknown");
        _userRepository.GetByRefreshTokenAsync("hashed-unknown").Returns((User?)null);

        // Act
        Result<AuthResult> result = await _handler.Handle(
            new RefreshTokenCommand("unknown-token"), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Unauthorized);
    }

    [Fact]
    public async Task Handle_WithExpiredToken_ReturnsUnauthorized()
    {
        // Arrange
        RefreshToken expiredToken = new()
        {
            Token = "hashed-expired",
            ExpiresAt = DateTime.UtcNow.AddDays(-1)
        };
        User user = new() { RefreshTokens = [expiredToken] };

        _tokenService.HashRefreshToken("expired-token").Returns("hashed-expired");
        _userRepository.GetByRefreshTokenAsync("hashed-expired").Returns(user);

        // Act
        Result<AuthResult> result = await _handler.Handle(
            new RefreshTokenCommand("expired-token"), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Unauthorized);
    }

    [Fact]
    public async Task Handle_WithRevokedToken_RevokesAllSessionsAndReturnsUnauthorized()
    {
        // Arrange — token was already rotated (RevokedAt set), but not yet expired.
        RefreshToken revokedToken = new()
        {
            Token = "hashed-revoked",
            ExpiresAt = DateTime.UtcNow.AddDays(6),
            RevokedAt = DateTime.UtcNow.AddHours(-1)
        };
        RefreshToken activeToken = new()
        {
            Token = "hashed-active",
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };
        User user = new() { RefreshTokens = [revokedToken, activeToken] };

        _tokenService.HashRefreshToken("revoked-token").Returns("hashed-revoked");
        _userRepository.GetByRefreshTokenAsync("hashed-revoked").Returns(user);

        // Act
        Result<AuthResult> result = await _handler.Handle(
            new RefreshTokenCommand("revoked-token"), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Unauthorized);
        result.Error.Code.Should().Be("Auth.TokenReuse");

        // All active tokens must be revoked.
        activeToken.RevokedAt.Should().NotBeNull();
        await _userRepository.Received(1).UpdateAsync(user, CancellationToken.None);
    }
}
