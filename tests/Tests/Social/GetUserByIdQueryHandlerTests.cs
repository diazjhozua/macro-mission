using FluentAssertions;
using MacroMission.Application.Common.Interfaces;
using MacroMission.Application.Social.Queries.GetUserById;
using MacroMission.Application.Social.Results;
using MacroMission.Domain.Common;
using MacroMission.Domain.Users;
using MongoDB.Bson;
using NSubstitute;

namespace MacroMission.Tests.Social;

public sealed class GetUserByIdQueryHandlerTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly GetUserByIdQueryHandler _handler;

    public GetUserByIdQueryHandlerTests()
    {
        _handler = new GetUserByIdQueryHandler(_userRepository);
    }

    [Fact]
    public async Task Handle_ReturnsUserSummary_WhenUserExists()
    {
        ObjectId userId = ObjectId.GenerateNewId();
        User user = new() { Nickname = "johndoe", FirstName = "John", LastName = "Doe" };
        _userRepository.GetByIdAsync(userId.ToString()).Returns(user);

        Result<UserSummaryResult> result = await _handler.Handle(
            new GetUserByIdQuery(userId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Nickname.Should().Be("johndoe");
        result.Value.FirstName.Should().Be("John");
        result.Value.LastName.Should().Be("Doe");
    }

    [Fact]
    public async Task Handle_ReturnsNotFound_WhenUserDoesNotExist()
    {
        ObjectId userId = ObjectId.GenerateNewId();
        _userRepository.GetByIdAsync(userId.ToString()).Returns((User?)null);

        Result<UserSummaryResult> result = await _handler.Handle(
            new GetUserByIdQuery(userId), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("User.NotFound");
    }
}
