using FluentAssertions;
using MacroMission.Application.Auth.Commands.Register;
using MacroMission.Application.Common.Interfaces;
using MacroMission.Domain.Common;
using MacroMission.Domain.Users;
using NSubstitute;

namespace MacroMission.Tests.Auth;

public sealed class RegisterCommandHandlerTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IPasswordHasher _passwordHasher = Substitute.For<IPasswordHasher>();
    private readonly IEmailService _emailService = Substitute.For<IEmailService>();
    private readonly RegisterCommandHandler _handler;

    public RegisterCommandHandlerTests()
    {
        _handler = new RegisterCommandHandler(_userRepository, _passwordHasher, _emailService);
    }

    [Fact]
    public async Task Handle_WhenEmailIsAvailable_CreatesUserAndSendsVerificationEmail()
    {
        // Arrange
        RegisterCommand command = new("test@example.com", "Password1", "John", "Doe", "johnd");
        _userRepository.ExistsByEmailAsync("test@example.com").Returns(false);
        _passwordHasher.Hash(command.Password).Returns("hashed-password");

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _userRepository.Received(1).CreateAsync(
            Arg.Is<User>(u =>
                u.Email == "test@example.com" &&
                u.FirstName == "John" &&
                u.LastName == "Doe" &&
                u.Nickname == "johnd" &&
                !u.IsEmailVerified),
            Arg.Any<CancellationToken>());
        await _emailService.Received(1).SendEmailVerificationAsync(
            "test@example.com",
            Arg.Any<string>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenEmailIsTaken_ReturnsConflictError()
    {
        // Arrange
        RegisterCommand command = new("taken@example.com", "Password1", "John", "Doe", "johnd");
        _userRepository.ExistsByEmailAsync("taken@example.com").Returns(true);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Conflict);
        await _userRepository.DidNotReceive().CreateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_NormalizesEmailToLowercase()
    {
        // Arrange
        RegisterCommand command = new("TEST@EXAMPLE.COM", "Password1", "John", "Doe", "johnd");
        _userRepository.ExistsByEmailAsync("test@example.com").Returns(false);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _userRepository.Received(1).CreateAsync(
            Arg.Is<User>(u => u.Email == "test@example.com"),
            Arg.Any<CancellationToken>());
    }
}
