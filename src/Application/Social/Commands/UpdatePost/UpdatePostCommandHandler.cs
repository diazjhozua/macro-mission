using MacroMission.Application.Common.Interfaces;
using MacroMission.Application.Common.Messaging;
using MacroMission.Application.Social.Commands.CreatePost;
using MacroMission.Application.Social.Results;
using MacroMission.Domain.Common;
using MacroMission.Domain.Social;

namespace MacroMission.Application.Social.Commands.UpdatePost;

internal sealed class UpdatePostCommandHandler(
    IPostRepository postRepository) : ICommandHandler<UpdatePostCommand, PostResult>
{
    public async Task<Result<PostResult>> Handle(
        UpdatePostCommand command,
        CancellationToken cancellationToken)
    {
        Post? post = await postRepository.GetByIdAsync(command.PostId, cancellationToken);

        if (post is null)
            return Result<PostResult>.Failure(Error.NotFound("Post.NotFound", "Post not found."));

        if (post.AuthorId != command.RequesterId)
            return Result<PostResult>.Failure(Error.Forbidden("Post.Forbidden", "You can only edit your own posts."));

        post.Caption = command.Caption;
        post.Visibility = command.Visibility;
        post.UpdatedAt = DateTime.UtcNow;

        await postRepository.UpdateAsync(post, cancellationToken);

        return Result<PostResult>.Success(CreatePostCommandHandler.ToResult(post));
    }
}
