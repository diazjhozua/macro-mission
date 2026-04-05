using MacroMission.Application.Common.Interfaces;
using MacroMission.Application.Common.Messaging;
using MacroMission.Application.Social.Results;
using MacroMission.Domain.Common;
using MacroMission.Domain.Social;

namespace MacroMission.Application.Social.Commands.AddComment;

internal sealed class AddCommentCommandHandler(
    IPostRepository postRepository,
    ICommentRepository commentRepository) : ICommandHandler<AddCommentCommand, CommentResult>
{
    public async Task<Result<CommentResult>> Handle(
        AddCommentCommand command,
        CancellationToken cancellationToken)
    {
        Post? post = await postRepository.GetByIdAsync(command.PostId, cancellationToken);

        if (post is null)
            return Result<CommentResult>.Failure(Error.NotFound("Post.NotFound", "Post not found."));

        Comment comment = new()
        {
            PostId = command.PostId,
            AuthorId = command.AuthorId,
            Text = command.Text
        };

        await commentRepository.CreateAsync(comment, cancellationToken);
        await postRepository.IncrementCommentsAsync(command.PostId, 1, cancellationToken);

        return Result<CommentResult>.Success(ToResult(comment));
    }

    internal static CommentResult ToResult(Comment comment) => new(
        comment.Id.ToString(),
        comment.PostId.ToString(),
        comment.AuthorId.ToString(),
        comment.Text,
        comment.CreatedAt);
}
