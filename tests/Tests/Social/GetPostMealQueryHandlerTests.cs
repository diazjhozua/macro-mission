using FluentAssertions;
using MacroMission.Application.Common.Interfaces;
using MacroMission.Application.Meals.Results;
using MacroMission.Application.Social.Queries.GetPostMeal;
using MacroMission.Domain.Common;
using MacroMission.Domain.Meals;
using MacroMission.Domain.Social;
using MongoDB.Bson;
using NSubstitute;

namespace MacroMission.Tests.Social;

public sealed class GetPostMealQueryHandlerTests
{
    private readonly IPostRepository _postRepository = Substitute.For<IPostRepository>();
    private readonly IFollowRepository _followRepository = Substitute.For<IFollowRepository>();
    private readonly IMealRepository _mealRepository = Substitute.For<IMealRepository>();
    private readonly GetPostMealQueryHandler _handler;
    private readonly ObjectId _requesterId = ObjectId.GenerateNewId();
    private readonly ObjectId _authorId = ObjectId.GenerateNewId();

    public GetPostMealQueryHandlerTests()
    {
        _handler = new GetPostMealQueryHandler(_postRepository, _followRepository, _mealRepository);
    }

    [Fact]
    public async Task Handle_ReturnsMeal_WhenPostIsPublic()
    {
        Post post = new() { AuthorId = _authorId, Visibility = PostVisibility.Public };
        Meal meal = new() { UserId = _authorId, MealType = MealType.Dinner };
        _postRepository.GetByIdAsync(post.Id).Returns(post);
        _mealRepository.GetByIdAsync(post.MealId).Returns(meal);

        Result<MealResult> result = await _handler.Handle(
            new GetPostMealQuery(post.Id, _requesterId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.MealType.Should().Be(MealType.Dinner);
    }

    [Fact]
    public async Task Handle_ReturnsMeal_WhenRequesterIsAuthor()
    {
        Post post = new() { AuthorId = _requesterId, Visibility = PostVisibility.Private };
        Meal meal = new() { UserId = _requesterId };
        _postRepository.GetByIdAsync(post.Id).Returns(post);
        _mealRepository.GetByIdAsync(post.MealId).Returns(meal);

        Result<MealResult> result = await _handler.Handle(
            new GetPostMealQuery(post.Id, _requesterId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ReturnsMeal_WhenPostIsFollowersOnlyAndRequesterFollows()
    {
        Post post = new() { AuthorId = _authorId, Visibility = PostVisibility.FollowersOnly };
        Meal meal = new() { UserId = _authorId };
        _postRepository.GetByIdAsync(post.Id).Returns(post);
        _followRepository.IsFollowingAsync(_requesterId, _authorId).Returns(true);
        _mealRepository.GetByIdAsync(post.MealId).Returns(meal);

        Result<MealResult> result = await _handler.Handle(
            new GetPostMealQuery(post.Id, _requesterId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ReturnsForbidden_WhenPostIsPrivate()
    {
        Post post = new() { AuthorId = _authorId, Visibility = PostVisibility.Private };
        _postRepository.GetByIdAsync(post.Id).Returns(post);

        Result<MealResult> result = await _handler.Handle(
            new GetPostMealQuery(post.Id, _requesterId), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Post.Forbidden");
        await _mealRepository.DidNotReceive().GetByIdAsync(Arg.Any<ObjectId>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ReturnsForbidden_WhenPostIsFollowersOnlyAndRequesterDoesNotFollow()
    {
        Post post = new() { AuthorId = _authorId, Visibility = PostVisibility.FollowersOnly };
        _postRepository.GetByIdAsync(post.Id).Returns(post);
        _followRepository.IsFollowingAsync(_requesterId, _authorId).Returns(false);

        Result<MealResult> result = await _handler.Handle(
            new GetPostMealQuery(post.Id, _requesterId), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Post.Forbidden");
    }

    [Fact]
    public async Task Handle_ReturnsNotFound_WhenPostDoesNotExist()
    {
        ObjectId postId = ObjectId.GenerateNewId();
        _postRepository.GetByIdAsync(postId).Returns((Post?)null);

        Result<MealResult> result = await _handler.Handle(
            new GetPostMealQuery(postId, _requesterId), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Post.NotFound");
    }
}
