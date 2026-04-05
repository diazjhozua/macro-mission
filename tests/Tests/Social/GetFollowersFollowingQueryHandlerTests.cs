using FluentAssertions;
using MacroMission.Application.Common.Interfaces;
using MacroMission.Application.Social.Queries.GetFollowers;
using MacroMission.Application.Social.Queries.GetFollowing;
using MacroMission.Application.Social.Results;
using MacroMission.Domain.Common;
using MacroMission.Domain.Users;
using MongoDB.Bson;
using NSubstitute;

namespace MacroMission.Tests.Social;

public sealed class GetFollowersFollowingQueryHandlerTests
{
    private readonly IFollowRepository _followRepository = Substitute.For<IFollowRepository>();
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly ObjectId _userId = ObjectId.GenerateNewId();

    [Fact]
    public async Task GetFollowers_ReturnsUsersWhoFollowTheRequestedUser()
    {
        // Arrange
        ObjectId followerId = ObjectId.GenerateNewId();
        _followRepository.GetFollowerIdsAsync(_userId).Returns([followerId]);
        _userRepository.GetByIdsAsync(Arg.Any<List<ObjectId>>()).Returns([
            new User { Nickname = "follower1", FirstName = "John", LastName = "Doe" }
        ]);

        GetFollowersQueryHandler handler = new(_followRepository, _userRepository);

        // Act
        Result<List<UserSummaryResult>> result = await handler.Handle(
            new GetFollowersQuery(_userId), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value[0].Nickname.Should().Be("follower1");
    }

    [Fact]
    public async Task GetFollowers_WhenNoFollowers_ReturnsEmptyList()
    {
        // Arrange
        _followRepository.GetFollowerIdsAsync(_userId).Returns([]);

        GetFollowersQueryHandler handler = new(_followRepository, _userRepository);

        // Act
        Result<List<UserSummaryResult>> result = await handler.Handle(
            new GetFollowersQuery(_userId), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
        await _userRepository.DidNotReceive().GetByIdsAsync(Arg.Any<List<ObjectId>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetFollowing_ReturnsUsersTheRequestedUserFollows()
    {
        // Arrange
        ObjectId followingId = ObjectId.GenerateNewId();
        _followRepository.GetFollowingIdsAsync(_userId).Returns([followingId]);
        _userRepository.GetByIdsAsync(Arg.Any<List<ObjectId>>()).Returns([
            new User { Nickname = "following1", FirstName = "Jane", LastName = "Smith" }
        ]);

        GetFollowingQueryHandler handler = new(_followRepository, _userRepository);

        // Act
        Result<List<UserSummaryResult>> result = await handler.Handle(
            new GetFollowingQuery(_userId), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value[0].Nickname.Should().Be("following1");
    }

    [Fact]
    public async Task GetFollowing_WhenNotFollowingAnyone_ReturnsEmptyList()
    {
        // Arrange
        _followRepository.GetFollowingIdsAsync(_userId).Returns([]);

        GetFollowingQueryHandler handler = new(_followRepository, _userRepository);

        // Act
        Result<List<UserSummaryResult>> result = await handler.Handle(
            new GetFollowingQuery(_userId), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
        await _userRepository.DidNotReceive().GetByIdsAsync(Arg.Any<List<ObjectId>>(), Arg.Any<CancellationToken>());
    }
}
