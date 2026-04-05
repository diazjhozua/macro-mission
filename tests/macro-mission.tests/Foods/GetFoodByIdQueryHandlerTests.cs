using FluentAssertions;
using MacroMission.Application.Common.Interfaces;
using MacroMission.Application.Foods.Queries.GetFoodById;
using MacroMission.Application.Foods.Results;
using MacroMission.Domain.Common;
using MacroMission.Domain.Foods;
using MongoDB.Bson;
using NSubstitute;

namespace MacroMission.Tests.Foods;

public sealed class GetFoodByIdQueryHandlerTests
{
    private readonly IFoodRepository _repository = Substitute.For<IFoodRepository>();
    private readonly GetFoodByIdQueryHandler _handler;
    private readonly ObjectId _userId = ObjectId.GenerateNewId();

    public GetFoodByIdQueryHandlerTests()
    {
        _handler = new GetFoodByIdQueryHandler(_repository);
    }

    [Fact]
    public async Task Handle_WhenGlobalFoodExists_ReturnsFood()
    {
        // Global foods are visible to all users regardless of who's requesting.
        Food food = new() { OwnerId = null, Name = "Oats", Per100g = new FoodMacros { Calories = 389 } };
        _repository.GetByIdAsync(food.Id).Returns(food);

        Result<FoodResult> result = await _handler.Handle(
            new GetFoodByIdQuery(food.Id, _userId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("Oats");
        result.Value.IsCustom.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_WhenOwnCustomFoodExists_ReturnsFood()
    {
        // Arrange
        Food food = new() { OwnerId = _userId, Name = "My Protein Shake", Per100g = new FoodMacros() };
        _repository.GetByIdAsync(food.Id).Returns(food);

        // Act
        Result<FoodResult> result = await _handler.Handle(
            new GetFoodByIdQuery(food.Id, _userId), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.IsCustom.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenFoodNotFound_ReturnsNotFoundError()
    {
        // Arrange
        _repository.GetByIdAsync(Arg.Any<ObjectId>()).Returns((Food?)null);

        // Act
        Result<FoodResult> result = await _handler.Handle(
            new GetFoodByIdQuery(ObjectId.GenerateNewId(), _userId), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task Handle_WhenCustomFoodBelongsToDifferentUser_ReturnsForbiddenError()
    {
        // Arrange
        Food food = new() { OwnerId = ObjectId.GenerateNewId(), Name = "Private Food", Per100g = new FoodMacros() };
        _repository.GetByIdAsync(food.Id).Returns(food);

        // Act
        Result<FoodResult> result = await _handler.Handle(
            new GetFoodByIdQuery(food.Id, _userId), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Forbidden);
    }
}
