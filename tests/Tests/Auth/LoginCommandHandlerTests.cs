using FluentAssertions;
using MacroMission.Application.Auth.Commands.Login;
using MacroMission.Application.Auth.Results;
using MacroMission.Application.Common.Interfaces;
using MacroMission.Domain.Common;
using MacroMission.Domain.Users;
using NSubstitute;

namespace MacroMission.Tests.Auth;

public sealed class LoginCommandHandlerTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IPasswordHasher _passwordHasher = Substitute.For<IPasswordHasher>();
    private readonly ITokenService _tokenService = Substitute.For<ITokenService>();
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _handler = new LoginCommandHandler(_userRepository, _passwordHasher, _tokenService);
    }

    [Fact]
    public async Task Handle_WithValidCredentials_ReturnsAuthResult()
    {
        // Arrange
        User user = new()
        {
            Email = "test@example.com",
            PasswordHash = "hashed-password",
            IsEmailVerified = true
        };
        _userRepository.GetByEmailAsync("test@example.com").Returns(user);
        _passwordHasher.Verify("Password1", "hashed-password").Returns(true);
        _tokenService.GenerateAccessToken(user).Returns("access-token");
        _tokenService.GenerateRefreshToken().Returns("raw-refresh-token");
        _tokenService.HashRefreshToken("raw-refresh-token").Returns("hashed-refresh-token");

        // Act
        Result<AuthResult> result = await _handler.Handle(
            new LoginCommand("test@example.com", "Password1"), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.AccessToken.Should().Be("access-token");
        result.Value.RefreshToken.Should().Be("raw-refresh-token");
    }

    [Fact]
    public async Task Handle_WithWrongPassword_ReturnsUnauthorizedError()
    {
        // Arrange
        User user = new()
        {
            Email = "test@example.com",
            PasswordHash = "hashed-password",
            IsEmailVerified = true
        };
        _userRepository.GetByEmailAsync("test@example.com").Returns(user);
        _passwordHasher.Verify("wrong-password", "hashed-password").Returns(false);

        // Act
        Result<AuthResult> result = await _handler.Handle(
            new LoginCommand("test@example.com", "wrong-password"), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Unauthorized);
    }

    [Fact]
    public async Task Handle_WithUnknownEmail_ReturnsUnauthorizedError()
    {
        // Generic error regardless of whether the email exists — avoids user enumeration.
        _userRepository.GetByEmailAsync(Arg.Any<string>()).Returns((User?)null);

        Result<AuthResult> result = await _handler.Handle(
            new LoginCommand("nobody@example.com", "Password1"), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Unauthorized);
    }

    [Fact]
    public async Task Handle_WithUnverifiedEmail_ReturnsForbiddenError()
    {
        // Arrange
        User user = new()
        {
            Email = "test@example.com",
            PasswordHash = "hashed-password",
            IsEmailVerified = false
        };
        _userRepository.GetByEmailAsync("test@example.com").Returns(user);
        _passwordHasher.Verify("Password1", "hashed-password").Returns(true);

        // Act
        Result<AuthResult> result = await _handler.Handle(
            new LoginCommand("test@example.com", "Password1"), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Forbidden);
    }
}
