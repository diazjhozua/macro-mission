using FluentAssertions;
using MacroMission.Application.Common.Interfaces;
using MacroMission.Application.Foods.Commands.UpdateFood;
using MacroMission.Application.Foods.Results;
using MacroMission.Domain.Common;
using MacroMission.Domain.Foods;
using MongoDB.Bson;
using NSubstitute;

namespace MacroMission.Tests.Foods;

public sealed class UpdateFoodCommandHandlerTests
{
    private readonly IFoodRepository _repository = Substitute.For<IFoodRepository>();
    private readonly UpdateFoodCommandHandler _handler;
    private readonly ObjectId _userId = ObjectId.GenerateNewId();

    public UpdateFoodCommandHandlerTests()
    {
        _handler = new UpdateFoodCommandHandler(_repository);
    }

    [Fact]
    public async Task Handle_WithOwnCustomFood_UpdatesSuccessfully()
    {
        // Arrange
        Food food = new() { OwnerId = _userId, Name = "Old Name", Per100g = new FoodMacros() };
        _repository.GetByIdAsync(food.Id).Returns(food);

        UpdateFoodCommand command = new(food.Id, _userId, "New Name", "Brand", 200, 20, 10, 5, 2);

        // Act
        Result<FoodResult> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("New Name");
        result.Value.Calories.Should().Be(200);
    }

    [Fact]
    public async Task Handle_WhenFoodNotFound_ReturnsNotFoundError()
    {
        // Arrange
        _repository.GetByIdAsync(Arg.Any<ObjectId>()).Returns((Food?)null);

        UpdateFoodCommand command = new(ObjectId.GenerateNewId(), _userId, "Name", null, 100, 10, 10, 5, 2);

        // Act
        Result<FoodResult> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task Handle_WhenFoodIsGlobal_ReturnsForbiddenError()
    {
        // Global food has no OwnerId — users cannot edit it.
        Food food = new() { OwnerId = null, Name = "Global Food", Per100g = new FoodMacros() };
        _repository.GetByIdAsync(food.Id).Returns(food);

        UpdateFoodCommand command = new(food.Id, _userId, "Hacked Name", null, 100, 10, 10, 5, 2);

        Result<FoodResult> result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Forbidden);
    }

    [Fact]
    public async Task Handle_WhenFoodBelongsToDifferentUser_ReturnsForbiddenError()
    {
        // Arrange
        Food food = new() { OwnerId = ObjectId.GenerateNewId(), Name = "Other Food", Per100g = new FoodMacros() };
        _repository.GetByIdAsync(food.Id).Returns(food);

        UpdateFoodCommand command = new(food.Id, _userId, "Name", null, 100, 10, 10, 5, 2);

        // Act
        Result<FoodResult> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Forbidden);
    }
}
