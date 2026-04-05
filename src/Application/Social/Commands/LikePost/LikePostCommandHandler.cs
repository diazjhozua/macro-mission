using MacroMission.Application.Common.Interfaces;
using MacroMission.Application.Common.Messaging;
using MacroMission.Domain.Common;
using MacroMission.Domain.Social;

namespace MacroMission.Application.Social.Commands.LikePost;

internal sealed class LikePostCommandHandler(
    IPostRepository postRepository,
    ILikeRepository likeRepository) : ICommandHandler<LikePostCommand>
{
    public async Task<Result> Handle(LikePostCommand command, CancellationToken cancellationToken)
    {
        Post? post = await postRepository.GetByIdAsync(command.PostId, cancellationToken);

        if (post is null)
            return Result.Failure(Error.NotFound("Post.NotFound", "Post not found."));

        Like? existing = await likeRepository.GetAsync(command.UserId, command.PostId, cancellationToken);

        if (existing is not null)
            return Result.Failure(Error.Conflict("Like.AlreadyLiked", "You have already liked this post."));

        Like like = new() { UserId = command.UserId, PostId = command.PostId };

        await likeRepository.CreateAsync(like, cancellationToken);

        // Increment counter on the post document — avoids counting likes on every read.
        await postRepository.IncrementLikesAsync(command.PostId, 1, cancellationToken);

        return Result.Success();
    }
}
