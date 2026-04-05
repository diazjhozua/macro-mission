using FluentAssertions;
using MacroMission.Application.Common.Interfaces;
using MacroMission.Application.Foods.Commands.CreateFood;
using MacroMission.Application.Foods.Results;
using MacroMission.Domain.Common;
using MacroMission.Domain.Foods;
using MongoDB.Bson;
using NSubstitute;

namespace MacroMission.Tests.Foods;

public sealed class CreateFoodCommandHandlerTests
{
    private readonly IFoodRepository _repository = Substitute.For<IFoodRepository>();
    private readonly CreateFoodCommandHandler _handler;
    private readonly ObjectId _userId = ObjectId.GenerateNewId();

    public CreateFoodCommandHandlerTests()
    {
        _handler = new CreateFoodCommandHandler(_repository);
    }

    [Fact]
    public async Task Handle_CreatesCustomFoodScopedToUser()
    {
        // Arrange
        CreateFoodCommand command = new(_userId, "Chicken Breast", "Brand X", 165, 31, 0, 3.6, 0);

        // Act
        Result<FoodResult> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("Chicken Breast");
        result.Value.IsCustom.Should().BeTrue();
        result.Value.Calories.Should().Be(165);
        await _repository.Received(1).CreateAsync(
            Arg.Is<Food>(f =>
                f.OwnerId == _userId &&
                f.Name == "Chicken Breast" &&
                f.Brand == "Brand X"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithNoBrand_CreatesFoodWithNullBrand()
    {
        // Arrange
        CreateFoodCommand command = new(_userId, "Rice", null, 130, 2.7, 28, 0.3, 0.4);

        // Act
        Result<FoodResult> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Brand.Should().BeNull();
    }
}
