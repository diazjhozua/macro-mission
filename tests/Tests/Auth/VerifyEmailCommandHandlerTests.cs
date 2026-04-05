using FluentAssertions;
using MacroMission.Application.Auth.Commands.VerifyEmail;
using MacroMission.Application.Common.Interfaces;
using MacroMission.Domain.Common;
using MacroMission.Domain.Users;
using NSubstitute;

namespace MacroMission.Tests.Auth;

public sealed class VerifyEmailCommandHandlerTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly VerifyEmailCommandHandler _handler;

    public VerifyEmailCommandHandlerTests()
    {
        _handler = new VerifyEmailCommandHandler(_userRepository);
    }

    [Fact]
    public async Task Handle_WithValidToken_VerifiesEmailAndClearsToken()
    {
        // Arrange
        User user = new()
        {
            Email = "test@example.com",
            EmailVerificationToken = "valid-token",
            EmailVerificationTokenExpiresAt = DateTime.UtcNow.AddHours(1),
            IsEmailVerified = false
        };
        _userRepository.GetByEmailVerificationTokenAsync("valid-token").Returns(user);

        // Act
        Result result = await _handler.Handle(
            new VerifyEmailCommand("valid-token"), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.IsEmailVerified.Should().BeTrue();
        user.EmailVerificationToken.Should().BeNull();
        user.EmailVerificationTokenExpiresAt.Should().BeNull();
        await _userRepository.Received(1).UpdateAsync(user, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithInvalidToken_ReturnsNotFoundError()
    {
        // Arrange
        _userRepository.GetByEmailVerificationTokenAsync("bad-token").Returns((User?)null);

        // Act
        Result result = await _handler.Handle(
            new VerifyEmailCommand("bad-token"), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task Handle_WithExpiredToken_ReturnsValidationError()
    {
        // Arrange
        User user = new()
        {
            Email = "test@example.com",
            EmailVerificationToken = "expired-token",
            EmailVerificationTokenExpiresAt = DateTime.UtcNow.AddHours(-1),
            IsEmailVerified = false
        };
        _userRepository.GetByEmailVerificationTokenAsync("expired-token").Returns(user);

        // Act
        Result result = await _handler.Handle(
            new VerifyEmailCommand("expired-token"), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public async Task Handle_WhenAlreadyVerified_ReturnsConflictError()
    {
        // Arrange
        User user = new()
        {
            Email = "test@example.com",
            EmailVerificationToken = "some-token",
            EmailVerificationTokenExpiresAt = DateTime.UtcNow.AddHours(1),
            IsEmailVerified = true
        };
        _userRepository.GetByEmailVerificationTokenAsync("some-token").Returns(user);

        // Act
        Result result = await _handler.Handle(
            new VerifyEmailCommand("some-token"), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Conflict);
    }
}
