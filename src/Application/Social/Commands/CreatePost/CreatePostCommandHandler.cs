using MacroMission.Application.Common.Interfaces;
using MacroMission.Application.Common.Messaging;
using MacroMission.Application.Social.Results;
using MacroMission.Domain.Common;
using MacroMission.Domain.Meals;
using MacroMission.Domain.Social;

namespace MacroMission.Application.Social.Commands.CreatePost;

internal sealed class CreatePostCommandHandler(
    IPostRepository postRepository,
    IMealRepository mealRepository) : ICommandHandler<CreatePostCommand, PostResult>
{
    public async Task<Result<PostResult>> Handle(
        CreatePostCommand command,
        CancellationToken cancellationToken)
    {
        Meal? meal = await mealRepository.GetByIdAsync(command.MealId, cancellationToken);

        if (meal is null)
            return Result<PostResult>.Failure(Error.NotFound("Post.MealNotFound", "Meal not found."));

        if (meal.UserId != command.AuthorId)
            return Result<PostResult>.Failure(Error.Forbidden("Post.Forbidden", "You can only post your own meals."));

        Post post = new()
        {
            AuthorId = command.AuthorId,
            MealId = command.MealId,
            Caption = command.Caption,
            Visibility = command.Visibility
        };

        await postRepository.CreateAsync(post, cancellationToken);

        return Result<PostResult>.Success(ToResult(post));
    }

    internal static PostResult ToResult(Post post) => new(
        post.Id.ToString(),
        post.AuthorId.ToString(),
        post.MealId.ToString(),
        post.Caption,
        post.Visibility,
        post.LikesCount,
        post.CommentsCount,
        post.CreatedAt,
        post.UpdatedAt);
}
