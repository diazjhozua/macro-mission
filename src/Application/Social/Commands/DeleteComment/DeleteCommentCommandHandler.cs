using MacroMission.Application.Common.Interfaces;
using MacroMission.Application.Common.Messaging;
using MacroMission.Domain.Common;
using MacroMission.Domain.Social;

namespace MacroMission.Application.Social.Commands.DeleteComment;

internal sealed class DeleteCommentCommandHandler(
    ICommentRepository commentRepository,
    IPostRepository postRepository) : ICommandHandler<DeleteCommentCommand>
{
    public async Task<Result> Handle(DeleteCommentCommand command, CancellationToken cancellationToken)
    {
        Comment? comment = await commentRepository.GetByIdAsync(command.CommentId, cancellationToken);

        if (comment is null)
            return Result.Failure(Error.NotFound("Comment.NotFound", "Comment not found."));

        if (comment.AuthorId != command.RequesterId)
            return Result.Failure(Error.Forbidden("Comment.Forbidden", "You can only delete your own comments."));

        await commentRepository.DeleteAsync(command.CommentId, cancellationToken);
        await postRepository.IncrementCommentsAsync(comment.PostId, -1, cancellationToken);

        return Result.Success();
    }
}
