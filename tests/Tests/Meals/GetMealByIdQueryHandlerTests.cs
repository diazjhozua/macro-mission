using FluentAssertions;
using MacroMission.Application.Common.Interfaces;
using MacroMission.Application.Meals.Queries.GetMealById;
using MacroMission.Application.Meals.Results;
using MacroMission.Domain.Common;
using MacroMission.Domain.Meals;
using MongoDB.Bson;
using NSubstitute;

namespace MacroMission.Tests.Meals;

public sealed class GetMealByIdQueryHandlerTests
{
    private readonly IMealRepository _repository = Substitute.For<IMealRepository>();
    private readonly GetMealByIdQueryHandler _handler;
    private readonly ObjectId _userId = ObjectId.GenerateNewId();

    public GetMealByIdQueryHandlerTests()
    {
        _handler = new GetMealByIdQueryHandler(_repository);
    }

    [Fact]
    public async Task Handle_ReturnsMealResult_WhenMealExistsAndBelongsToUser()
    {
        Meal meal = new() { UserId = _userId, MealType = MealType.Lunch };
        _repository.GetByIdAsync(meal.Id).Returns(meal);

        Result<MealResult> result = await _handler.Handle(
            new GetMealByIdQuery(meal.Id, _userId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(meal.Id.ToString());
        result.Value.MealType.Should().Be(MealType.Lunch);
    }

    [Fact]
    public async Task Handle_ReturnsNotFound_WhenMealDoesNotExist()
    {
        ObjectId mealId = ObjectId.GenerateNewId();
        _repository.GetByIdAsync(mealId).Returns((Meal?)null);

        Result<MealResult> result = await _handler.Handle(
            new GetMealByIdQuery(mealId, _userId), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Meal.NotFound");
    }

    [Fact]
    public async Task Handle_ReturnsForbidden_WhenMealBelongsToDifferentUser()
    {
        ObjectId otherUserId = ObjectId.GenerateNewId();
        Meal meal = new() { UserId = otherUserId };
        _repository.GetByIdAsync(meal.Id).Returns(meal);

        Result<MealResult> result = await _handler.Handle(
            new GetMealByIdQuery(meal.Id, _userId), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Meal.Forbidden");
    }
}
