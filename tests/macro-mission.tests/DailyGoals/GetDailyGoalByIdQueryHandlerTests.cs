using ErrorOr;
using FluentAssertions;
using MacroMission.Application.Common.Interfaces;
using MacroMission.Application.DailyGoals.Queries.GetDailyGoalById;
using MacroMission.Application.DailyGoals.Results;
using MacroMission.Domain.DailyGoals;
using MongoDB.Bson;
using NSubstitute;

namespace MacroMission.Tests.DailyGoals;

public sealed class GetDailyGoalByIdQueryHandlerTests
{
    private readonly IDailyGoalRepository _repository = Substitute.For<IDailyGoalRepository>();
    private readonly GetDailyGoalByIdQueryHandler _handler;
    private readonly ObjectId _userId = ObjectId.GenerateNewId();

    public GetDailyGoalByIdQueryHandlerTests()
    {
        _handler = new GetDailyGoalByIdQueryHandler(_repository);
    }

    [Fact]
    public async Task Handle_WhenGoalExists_ReturnsGoal()
    {
        // Arrange
        DailyGoal goal = new() { UserId = _userId, Name = "Cut", IsActive = true };
        _repository.GetByIdAsync(goal.Id).Returns(goal);

        // Act
        ErrorOr<DailyGoalResult> result = await _handler.Handle(
            new GetDailyGoalByIdQuery(goal.Id, _userId), CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Name.Should().Be("Cut");
        result.Value.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenGoalNotFound_ReturnsNotFoundError()
    {
        // Arrange
        _repository.GetByIdAsync(Arg.Any<ObjectId>()).Returns((DailyGoal?)null);

        // Act
        ErrorOr<DailyGoalResult> result = await _handler.Handle(
            new GetDailyGoalByIdQuery(ObjectId.GenerateNewId(), _userId), CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task Handle_WhenGoalBelongsToDifferentUser_ReturnsForbiddenError()
    {
        // Arrange
        DailyGoal goal = new() { UserId = ObjectId.GenerateNewId() }; // different owner
        _repository.GetByIdAsync(goal.Id).Returns(goal);

        // Act
        ErrorOr<DailyGoalResult> result = await _handler.Handle(
            new GetDailyGoalByIdQuery(goal.Id, _userId), CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Type.Should().Be(ErrorType.Forbidden);
    }
}
