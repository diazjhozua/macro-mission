using FluentAssertions;
using MacroMission.Application.Common.Interfaces;
using MacroMission.Application.Social.Commands.CreatePost;
using MacroMission.Application.Social.Results;
using MacroMission.Domain.Common;
using MacroMission.Domain.Meals;
using MacroMission.Domain.Social;
using MongoDB.Bson;
using NSubstitute;

namespace MacroMission.Tests.Social;

public sealed class CreatePostCommandHandlerTests
{
    private readonly IPostRepository _postRepository = Substitute.For<IPostRepository>();
    private readonly IMealRepository _mealRepository = Substitute.For<IMealRepository>();
    private readonly CreatePostCommandHandler _handler;
    private readonly ObjectId _userId = ObjectId.GenerateNewId();

    public CreatePostCommandHandlerTests()
    {
        _handler = new CreatePostCommandHandler(_postRepository, _mealRepository);
    }

    [Fact]
    public async Task Handle_WithOwnMeal_CreatesPost()
    {
        // Arrange
        Meal meal = new() { UserId = _userId };
        _mealRepository.GetByIdAsync(meal.Id).Returns(meal);

        CreatePostCommand command = new(_userId, meal.Id, "My meal!", PostVisibility.Public);

        // Act
        Result<PostResult> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Caption.Should().Be("My meal!");
        result.Value.Visibility.Should().Be(PostVisibility.Public);
        await _postRepository.Received(1).CreateAsync(Arg.Any<Post>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenMealNotFound_ReturnsNotFoundError()
    {
        _mealRepository.GetByIdAsync(Arg.Any<ObjectId>()).Returns((Meal?)null);

        Result<PostResult> result = await _handler.Handle(
            new CreatePostCommand(_userId, ObjectId.GenerateNewId(), null, PostVisibility.Public),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task Handle_WhenMealBelongsToDifferentUser_ReturnsForbiddenError()
    {
        Meal meal = new() { UserId = ObjectId.GenerateNewId() };
        _mealRepository.GetByIdAsync(meal.Id).Returns(meal);

        Result<PostResult> result = await _handler.Handle(
            new CreatePostCommand(_userId, meal.Id, null, PostVisibility.Public),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Forbidden);
    }
}
