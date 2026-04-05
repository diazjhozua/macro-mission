using MacroMission.Application.Common.Interfaces;
using MacroMission.Application.Common.Messaging;
using MacroMission.Domain.Common;
using MacroMission.Domain.Social;

namespace MacroMission.Application.Social.Commands.UnlikePost;

internal sealed class UnlikePostCommandHandler(
    IPostRepository postRepository,
    ILikeRepository likeRepository) : ICommandHandler<UnlikePostCommand>
{
    public async Task<Result> Handle(UnlikePostCommand command, CancellationToken cancellationToken)
    {
        Post? post = await postRepository.GetByIdAsync(command.PostId, cancellationToken);

        if (post is null)
            return Result.Failure(Error.NotFound("Post.NotFound", "Post not found."));

        Like? existing = await likeRepository.GetAsync(command.UserId, command.PostId, cancellationToken);

        if (existing is null)
            return Result.Failure(Error.NotFound("Like.NotLiked", "You have not liked this post."));

        await likeRepository.DeleteAsync(command.UserId, command.PostId, cancellationToken);
        await postRepository.IncrementLikesAsync(command.PostId, -1, cancellationToken);

        return Result.Success();
    }
}
