using MacroMission.Application.Common.Interfaces;
using MacroMission.Application.Common.Messaging;
using MacroMission.Application.Meals.Commands.CreateMeal;
using MacroMission.Application.Meals.Results;
using MacroMission.Domain.Common;
using MacroMission.Domain.Meals;
using MacroMission.Domain.Social;

namespace MacroMission.Application.Social.Queries.GetPostMeal;

internal sealed class GetPostMealQueryHandler(
    IPostRepository postRepository,
    IFollowRepository followRepository,
    IMealRepository mealRepository) : IQueryHandler<GetPostMealQuery, MealResult>
{
    public async Task<Result<MealResult>> Handle(
        GetPostMealQuery query,
        CancellationToken cancellationToken)
    {
        Post? post = await postRepository.GetByIdAsync(query.PostId, cancellationToken);

        if (post is null)
            return Result<MealResult>.Failure(Error.NotFound("Post.NotFound", "Post not found."));

        if (post.AuthorId != query.RequesterId)
        {
            if (post.Visibility == PostVisibility.Private)
                return Result<MealResult>.Failure(Error.Forbidden("Post.Forbidden", "This post is private."));

            if (post.Visibility == PostVisibility.FollowersOnly)
            {
                bool isFollowing = await followRepository.IsFollowingAsync(
                    query.RequesterId, post.AuthorId, cancellationToken);

                if (!isFollowing)
                    return Result<MealResult>.Failure(Error.Forbidden("Post.Forbidden", "This post is for followers only."));
            }
        }

        Meal? meal = await mealRepository.GetByIdAsync(post.MealId, cancellationToken);

        if (meal is null)
            return Result<MealResult>.Failure(Error.NotFound("Meal.NotFound", "Meal not found."));

        return Result<MealResult>.Success(CreateMealCommandHandler.ToResult(meal));
    }
}
