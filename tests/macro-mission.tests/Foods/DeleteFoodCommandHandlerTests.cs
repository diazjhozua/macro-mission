using FluentAssertions;
using MacroMission.Application.Common.Interfaces;
using MacroMission.Application.Foods.Commands.DeleteFood;
using MacroMission.Domain.Common;
using MacroMission.Domain.Foods;
using MongoDB.Bson;
using NSubstitute;

namespace MacroMission.Tests.Foods;

public sealed class DeleteFoodCommandHandlerTests
{
    private readonly IFoodRepository _repository = Substitute.For<IFoodRepository>();
    private readonly DeleteFoodCommandHandler _handler;
    private readonly ObjectId _userId = ObjectId.GenerateNewId();

    public DeleteFoodCommandHandlerTests()
    {
        _handler = new DeleteFoodCommandHandler(_repository);
    }

    [Fact]
    public async Task Handle_WithOwnCustomFood_DeletesSuccessfully()
    {
        // Arrange
        Food food = new() { OwnerId = _userId };
        _repository.GetByIdAsync(food.Id).Returns(food);

        // Act
        Result result = await _handler.Handle(
            new DeleteFoodCommand(food.Id, _userId), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _repository.Received(1).DeleteAsync(food.Id, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenFoodNotFound_ReturnsNotFoundError()
    {
        // Arrange
        _repository.GetByIdAsync(Arg.Any<ObjectId>()).Returns((Food?)null);

        // Act
        Result result = await _handler.Handle(
            new DeleteFoodCommand(ObjectId.GenerateNewId(), _userId), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.NotFound);
        await _repository.DidNotReceive().DeleteAsync(Arg.Any<ObjectId>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenFoodIsGlobal_ReturnsForbiddenError()
    {
        // Global food — no owner, users cannot delete it.
        Food food = new() { OwnerId = null };
        _repository.GetByIdAsync(food.Id).Returns(food);

        Result result = await _handler.Handle(
            new DeleteFoodCommand(food.Id, _userId), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Forbidden);
        await _repository.DidNotReceive().DeleteAsync(Arg.Any<ObjectId>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenFoodBelongsToDifferentUser_ReturnsForbiddenError()
    {
        // Arrange
        Food food = new() { OwnerId = ObjectId.GenerateNewId() };
        _repository.GetByIdAsync(food.Id).Returns(food);

        // Act
        Result result = await _handler.Handle(
            new DeleteFoodCommand(food.Id, _userId), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Forbidden);
        await _repository.DidNotReceive().DeleteAsync(Arg.Any<ObjectId>(), Arg.Any<CancellationToken>());
    }
}
