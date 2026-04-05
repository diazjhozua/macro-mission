using FluentAssertions;
using MacroMission.Application.Common.Interfaces;
using MacroMission.Application.Social.Commands.Follow;
using MacroMission.Domain.Common;
using MacroMission.Domain.Social;
using MongoDB.Bson;
using NSubstitute;

namespace MacroMission.Tests.Social;

public sealed class FollowCommandHandlerTests
{
    private readonly IFollowRepository _followRepository = Substitute.For<IFollowRepository>();
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly FollowCommandHandler _handler;
    private readonly ObjectId _followerId = ObjectId.GenerateNewId();
    private readonly ObjectId _followingId = ObjectId.GenerateNewId();

    public FollowCommandHandlerTests()
    {
        _handler = new FollowCommandHandler(_followRepository, _userRepository);
    }

    [Fact]
    public async Task Handle_WhenValid_CreatesFollow()
    {
        // Arrange
        _userRepository.ExistsByIdAsync(_followingId).Returns(true);
        _followRepository.IsFollowingAsync(_followerId, _followingId).Returns(false);

        // Act
        Result result = await _handler.Handle(
            new FollowCommand(_followerId, _followingId), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _followRepository.Received(1).CreateAsync(
            Arg.Is<Domain.Social.Follow>(f =>
                f.FollowerId == _followerId && f.FollowingId == _followingId),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenSelfFollow_ReturnsValidationError()
    {
        Result result = await _handler.Handle(
            new FollowCommand(_followerId, _followerId), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ReturnsNotFoundError()
    {
        _userRepository.ExistsByIdAsync(_followingId).Returns(false);

        Result result = await _handler.Handle(
            new FollowCommand(_followerId, _followingId), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task Handle_WhenAlreadyFollowing_ReturnsConflictError()
    {
        _userRepository.ExistsByIdAsync(_followingId).Returns(true);
        _followRepository.IsFollowingAsync(_followerId, _followingId).Returns(true);

        Result result = await _handler.Handle(
            new FollowCommand(_followerId, _followingId), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Conflict);
    }
}
