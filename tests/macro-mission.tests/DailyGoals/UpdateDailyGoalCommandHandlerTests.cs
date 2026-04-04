using FluentAssertions;
using MacroMission.Application.Common.Interfaces;
using MacroMission.Application.DailyGoals.Commands.UpdateDailyGoal;
using MacroMission.Application.DailyGoals.Results;
using MacroMission.Domain.Common;
using MacroMission.Domain.DailyGoals;
using MongoDB.Bson;
using NSubstitute;

namespace MacroMission.Tests.DailyGoals;

public sealed class UpdateDailyGoalCommandHandlerTests
{
    private readonly IDailyGoalRepository _repository = Substitute.For<IDailyGoalRepository>();
    private readonly UpdateDailyGoalCommandHandler _handler;
    private readonly ObjectId _userId = ObjectId.GenerateNewId();

    public UpdateDailyGoalCommandHandlerTests()
    {
        _handler = new UpdateDailyGoalCommandHandler(_repository);
    }

    [Fact]
    public async Task Handle_WithValidGoal_UpdatesFields()
    {
        // Arrange
        DailyGoal goal = new() { UserId = _userId, Name = "Old Name", IsActive = false };
        _repository.GetByIdAsync(goal.Id).Returns(goal);
        UpdateDailyGoalCommand command = new(goal.Id, _userId, "New Name", false, 2000, 150, 200, 65, 30);

        // Act
        Result<DailyGoalResult> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("New Name");
        result.Value.Calories.Should().Be(2000);
    }

    [Fact]
    public async Task Handle_WhenSettingActive_DeactivatesOthersFirst()
    {
        // Arrange
        DailyGoal goal = new() { UserId = _userId, IsActive = false };
        _repository.GetByIdAsync(goal.Id).Returns(goal);
        UpdateDailyGoalCommand command = new(goal.Id, _userId, "Bulk", true, 2800, 180, 300, 90, 25);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert — deactivation must happen before the update.
        await _repository.Received(1).DeactivateAllByUserIdAsync(_userId, Arg.Any<CancellationToken>());
        await _repository.Received(1).UpdateAsync(
            Arg.Is<DailyGoal>(g => g.IsActive),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenGoalNotFound_ReturnsNotFoundError()
    {
        // Arrange
        _repository.GetByIdAsync(Arg.Any<ObjectId>()).Returns((DailyGoal?)null);
        UpdateDailyGoalCommand command = new(ObjectId.GenerateNewId(), _userId, "Name", false, 2000, 150, 200, 65, 30);

        // Act
        Result<DailyGoalResult> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task Handle_WhenGoalBelongsToDifferentUser_ReturnsForbiddenError()
    {
        // Arrange
        DailyGoal goal = new() { UserId = ObjectId.GenerateNewId() };
        _repository.GetByIdAsync(goal.Id).Returns(goal);
        UpdateDailyGoalCommand command = new(goal.Id, _userId, "Name", false, 2000, 150, 200, 65, 30);

        // Act
        Result<DailyGoalResult> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Forbidden);
    }
}
