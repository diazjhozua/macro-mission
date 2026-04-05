using FluentAssertions;
using MacroMission.Application.Common.Interfaces;
using MacroMission.Application.Meals.Commands.CreateMeal;
using MacroMission.Application.Meals.Results;
using MacroMission.Domain.Common;
using MacroMission.Domain.Foods;
using MacroMission.Domain.Meals;
using MongoDB.Bson;
using NSubstitute;

namespace MacroMission.Tests.Meals;

public sealed class CreateMealCommandHandlerTests
{
    private readonly IMealRepository _mealRepository = Substitute.For<IMealRepository>();
    private readonly IFoodRepository _foodRepository = Substitute.For<IFoodRepository>();
    private readonly CreateMealCommandHandler _handler;
    private readonly ObjectId _userId = ObjectId.GenerateNewId();

    public CreateMealCommandHandlerTests()
    {
        _handler = new CreateMealCommandHandler(_mealRepository, _foodRepository);
    }

    [Fact]
    public async Task Handle_WithValidItems_ComputesMacrosCorrectly()
    {
        // Arrange — chicken breast: 165 kcal / 31g protein per 100g, eating 200g
        Food food = new()
        {
            Id = ObjectId.GenerateNewId(),
            Name = "Chicken Breast",
            Per100g = new FoodMacros { Calories = 165, Protein = 31, Carbs = 0, Fat = 3.6, Fiber = 0 }
        };
        _foodRepository.GetByIdAsync(food.Id).Returns(food);

        CreateMealCommand command = new(
            _userId,
            MealType.Lunch,
            null,
            [new CreateMealItemCommand(food.Id, 200)]);

        // Act
        Result<MealResult> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Totals.Calories.Should().Be(330); // 165 * 2
        result.Value.Totals.Protein.Should().Be(62);   // 31 * 2
        result.Value.Items.Should().HaveCount(1);
        result.Value.Items[0].FoodName.Should().Be("Chicken Breast");
        result.Value.Items[0].Grams.Should().Be(200);
    }

    [Fact]
    public async Task Handle_WhenFoodNotFound_ReturnsNotFoundError()
    {
        // Arrange
        ObjectId unknownFoodId = ObjectId.GenerateNewId();
        _foodRepository.GetByIdAsync(unknownFoodId).Returns((Food?)null);

        CreateMealCommand command = new(
            _userId,
            MealType.Breakfast,
            null,
            [new CreateMealItemCommand(unknownFoodId, 100)]);

        // Act
        Result<MealResult> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.NotFound);
        await _mealRepository.DidNotReceive().CreateAsync(Arg.Any<Meal>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_SumsTotalsAcrossMultipleItems()
    {
        // Arrange
        Food food1 = new()
        {
            Per100g = new FoodMacros { Calories = 100, Protein = 10, Carbs = 10, Fat = 5, Fiber = 1 }
        };
        Food food2 = new()
        {
            Per100g = new FoodMacros { Calories = 200, Protein = 20, Carbs = 20, Fat = 10, Fiber = 2 }
        };
        _foodRepository.GetByIdAsync(food1.Id).Returns(food1);
        _foodRepository.GetByIdAsync(food2.Id).Returns(food2);

        CreateMealCommand command = new(
            _userId,
            MealType.Dinner,
            null,
            [
                new CreateMealItemCommand(food1.Id, 100), // 100% of per100g
                new CreateMealItemCommand(food2.Id, 100)  // 100% of per100g
            ]);

        // Act
        Result<MealResult> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Totals.Calories.Should().Be(300);
        result.Value.Totals.Protein.Should().Be(30);
    }

    [Fact]
    public async Task Handle_DefaultsDateToTodayWhenNotProvided()
    {
        // Arrange
        Food food = new() { Per100g = new FoodMacros() };
        _foodRepository.GetByIdAsync(food.Id).Returns(food);

        CreateMealCommand command = new(_userId, MealType.Snack, null, [new CreateMealItemCommand(food.Id, 50)]);

        // Act
        Result<MealResult> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Date.Date.Should().Be(DateTime.UtcNow.Date);
    }
}
