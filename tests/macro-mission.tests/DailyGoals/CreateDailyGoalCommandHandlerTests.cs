using ErrorOr;
using FluentAssertions;
using MacroMission.Application.Common.Interfaces;
using MacroMission.Application.DailyGoals.Commands.CreateDailyGoal;
using MacroMission.Application.DailyGoals.Results;
using MacroMission.Domain.DailyGoals;
using MongoDB.Bson;
using NSubstitute;

namespace MacroMission.Tests.DailyGoals;

public sealed class CreateDailyGoalCommandHandlerTests
{
    private readonly IDailyGoalRepository _repository = Substitute.For<IDailyGoalRepository>();
    private readonly CreateDailyGoalCommandHandler _handler;
    private readonly ObjectId _userId = ObjectId.GenerateNewId();

    public CreateDailyGoalCommandHandlerTests()
    {
        _handler = new CreateDailyGoalCommandHandler(_repository);
    }

    [Fact]
    public async Task Handle_WhenNoActiveGoalExists_CreatesGoalAsActive()
    {
        // Arrange
        _repository.GetActiveByUserIdAsync(_userId).Returns((DailyGoal?)null);

        CreateDailyGoalCommand command = new(_userId, "Summer Cut", 1800, 160, 150, 55, 30);

        // Act
        ErrorOr<DailyGoalResult> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.IsActive.Should().BeTrue();
        result.Value.Name.Should().Be("Summer Cut");
    }

    [Fact]
    public async Task Handle_WhenActiveGoalExists_CreatesGoalAsInactive()
    {
        // Arrange
        _repository.GetActiveByUserIdAsync(_userId).Returns(new DailyGoal { IsActive = true });

        CreateDailyGoalCommand command = new(_userId, "Winter Bulk", 2800, 180, 300, 90, 25);

        // Act
        ErrorOr<DailyGoalResult> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_PersistsGoalWithCorrectMacros()
    {
        // Arrange
        _repository.GetActiveByUserIdAsync(_userId).Returns((DailyGoal?)null);
        CreateDailyGoalCommand command = new(_userId, "Maintenance", 2200, 170, 220, 70, 35);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _repository.Received(1).CreateAsync(
            Arg.Is<DailyGoal>(g =>
                g.UserId == _userId &&
                g.Calories == 2200 &&
                g.Protein == 170 &&
                g.Carbs == 220 &&
                g.Fat == 70 &&
                g.Fiber == 35),
            Arg.Any<CancellationToken>());
    }
}
