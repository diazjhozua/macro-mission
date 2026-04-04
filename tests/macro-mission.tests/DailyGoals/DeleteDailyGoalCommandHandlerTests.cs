using FluentAssertions;
using MacroMission.Application.Common.Interfaces;
using MacroMission.Application.DailyGoals.Commands.DeleteDailyGoal;
using MacroMission.Domain.Common;
using MacroMission.Domain.DailyGoals;
using MongoDB.Bson;
using NSubstitute;

namespace MacroMission.Tests.DailyGoals;

public sealed class DeleteDailyGoalCommandHandlerTests
{
    private readonly IDailyGoalRepository _repository = Substitute.For<IDailyGoalRepository>();
    private readonly DeleteDailyGoalCommandHandler _handler;
    private readonly ObjectId _userId = ObjectId.GenerateNewId();

    public DeleteDailyGoalCommandHandlerTests()
    {
        _handler = new DeleteDailyGoalCommandHandler(_repository);
    }

    [Fact]
    public async Task Handle_WhenGoalExists_DeletesAndReturnsSuccess()
    {
        // Arrange
        DailyGoal goal = new() { UserId = _userId };
        _repository.GetByIdAsync(goal.Id).Returns(goal);

        // Act
        Result result = await _handler.Handle(
            new DeleteDailyGoalCommand(goal.Id, _userId), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _repository.Received(1).DeleteAsync(goal.Id, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenGoalNotFound_ReturnsNotFoundError()
    {
        // Arrange
        _repository.GetByIdAsync(Arg.Any<ObjectId>()).Returns((DailyGoal?)null);

        // Act
        Result result = await _handler.Handle(
            new DeleteDailyGoalCommand(ObjectId.GenerateNewId(), _userId), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.NotFound);
        await _repository.DidNotReceive().DeleteAsync(Arg.Any<ObjectId>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenGoalBelongsToDifferentUser_ReturnsForbiddenError()
    {
        // Arrange
        DailyGoal goal = new() { UserId = ObjectId.GenerateNewId() }; // different owner
        _repository.GetByIdAsync(goal.Id).Returns(goal);

        // Act
        Result result = await _handler.Handle(
            new DeleteDailyGoalCommand(goal.Id, _userId), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Forbidden);
        await _repository.DidNotReceive().DeleteAsync(Arg.Any<ObjectId>(), Arg.Any<CancellationToken>());
    }
}
