using FluentAssertions;
using MacroMission.Application.Common.Interfaces;
using MacroMission.Application.Meals.Commands.DeleteMeal;
using MacroMission.Domain.Common;
using MacroMission.Domain.Meals;
using MongoDB.Bson;
using NSubstitute;

namespace MacroMission.Tests.Meals;

public sealed class DeleteMealCommandHandlerTests
{
    private readonly IMealRepository _repository = Substitute.For<IMealRepository>();
    private readonly DeleteMealCommandHandler _handler;
    private readonly ObjectId _userId = ObjectId.GenerateNewId();

    public DeleteMealCommandHandlerTests()
    {
        _handler = new DeleteMealCommandHandler(_repository);
    }

    [Fact]
    public async Task Handle_WhenMealExists_DeletesSuccessfully()
    {
        // Arrange
        Meal meal = new() { UserId = _userId };
        _repository.GetByIdAsync(meal.Id).Returns(meal);

        // Act
        Result result = await _handler.Handle(
            new DeleteMealCommand(meal.Id, _userId), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _repository.Received(1).DeleteAsync(meal.Id, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenMealNotFound_ReturnsNotFoundError()
    {
        // Arrange
        _repository.GetByIdAsync(Arg.Any<ObjectId>()).Returns((Meal?)null);

        // Act
        Result result = await _handler.Handle(
            new DeleteMealCommand(ObjectId.GenerateNewId(), _userId), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.NotFound);
        await _repository.DidNotReceive().DeleteAsync(Arg.Any<ObjectId>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenMealBelongsToDifferentUser_ReturnsForbiddenError()
    {
        // Arrange
        Meal meal = new() { UserId = ObjectId.GenerateNewId() };
        _repository.GetByIdAsync(meal.Id).Returns(meal);

        // Act
        Result result = await _handler.Handle(
            new DeleteMealCommand(meal.Id, _userId), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Forbidden);
        await _repository.DidNotReceive().DeleteAsync(Arg.Any<ObjectId>(), Arg.Any<CancellationToken>());
    }
}
