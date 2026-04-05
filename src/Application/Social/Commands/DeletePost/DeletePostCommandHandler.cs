using MacroMission.Application.Common.Interfaces;
using MacroMission.Application.Common.Messaging;
using MacroMission.Domain.Common;
using MacroMission.Domain.Social;

namespace MacroMission.Application.Social.Commands.DeletePost;

internal sealed class DeletePostCommandHandler(
    IPostRepository postRepository) : ICommandHandler<DeletePostCommand>
{
    public async Task<Result> Handle(DeletePostCommand command, CancellationToken cancellationToken)
    {
        Post? post = await postRepository.GetByIdAsync(command.PostId, cancellationToken);

        if (post is null)
            return Result.Failure(Error.NotFound("Post.NotFound", "Post not found."));

        if (post.AuthorId != command.RequesterId)
            return Result.Failure(Error.Forbidden("Post.Forbidden", "You can only delete your own posts."));

        await postRepository.DeleteAsync(command.PostId, cancellationToken);

        return Result.Success();
    }
}
