using FluentAssertions;
using MacroMission.Application.Common.Interfaces;
using MacroMission.Application.Meals.Queries.GetDailySummary;
using MacroMission.Application.Meals.Results;
using MacroMission.Domain.Common;
using MacroMission.Domain.DailyGoals;
using MacroMission.Domain.Meals;
using MongoDB.Bson;
using NSubstitute;

namespace MacroMission.Tests.Meals;

public sealed class GetDailySummaryQueryHandlerTests
{
    private readonly IMealRepository _mealRepository = Substitute.For<IMealRepository>();
    private readonly IDailyGoalRepository _goalRepository = Substitute.For<IDailyGoalRepository>();
    private readonly GetDailySummaryQueryHandler _handler;
    private readonly ObjectId _userId = ObjectId.GenerateNewId();
    private readonly DateTime _today = DateTime.UtcNow.Date;

    public GetDailySummaryQueryHandlerTests()
    {
        _handler = new GetDailySummaryQueryHandler(_mealRepository, _goalRepository);
    }

    [Fact]
    public async Task Handle_SumsTotalsAcrossAllMealsForTheDay()
    {
        // Arrange
        List<Meal> meals =
        [
            new Meal { UserId = _userId, Date = _today, Totals = new MealMacros { Calories = 500, Protein = 40, Carbs = 50, Fat = 15, Fiber = 5 } },
            new Meal { UserId = _userId, Date = _today, Totals = new MealMacros { Calories = 700, Protein = 50, Carbs = 80, Fat = 20, Fiber = 8 } }
        ];
        _mealRepository.GetByDateAsync(_userId, _today).Returns(meals);
        _goalRepository.GetActiveByUserIdAsync(_userId).Returns((DailyGoal?)null);

        // Act
        Result<DailySummaryResult> result = await _handler.Handle(
            new GetDailySummaryQuery(_userId, _today), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Consumed.Calories.Should().Be(1200);
        result.Value.Consumed.Protein.Should().Be(90);
        result.Value.Meals.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_WhenActiveGoalExists_IncludesGoalInSummary()
    {
        // Arrange
        _mealRepository.GetByDateAsync(_userId, _today).Returns([]);
        DailyGoal goal = new() { UserId = _userId, IsActive = true, Calories = 2000, Protein = 150, Carbs = 200, Fat = 65, Fiber = 30 };
        _goalRepository.GetActiveByUserIdAsync(_userId).Returns(goal);

        // Act
        Result<DailySummaryResult> result = await _handler.Handle(
            new GetDailySummaryQuery(_userId, _today), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Goal.Should().NotBeNull();
        result.Value.Goal!.Calories.Should().Be(2000);
        result.Value.Goal.Protein.Should().Be(150);
    }

    [Fact]
    public async Task Handle_WhenNoActiveGoal_GoalIsNull()
    {
        // Arrange
        _mealRepository.GetByDateAsync(_userId, _today).Returns([]);
        _goalRepository.GetActiveByUserIdAsync(_userId).Returns((DailyGoal?)null);

        // Act
        Result<DailySummaryResult> result = await _handler.Handle(
            new GetDailySummaryQuery(_userId, _today), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Goal.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WhenNoMealsLogged_ConsumedIsZero()
    {
        // Arrange
        _mealRepository.GetByDateAsync(_userId, _today).Returns([]);
        _goalRepository.GetActiveByUserIdAsync(_userId).Returns((DailyGoal?)null);

        // Act
        Result<DailySummaryResult> result = await _handler.Handle(
            new GetDailySummaryQuery(_userId, _today), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Consumed.Calories.Should().Be(0);
        result.Value.Meals.Should().BeEmpty();
    }
}
