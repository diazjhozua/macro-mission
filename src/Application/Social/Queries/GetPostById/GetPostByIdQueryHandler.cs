using MacroMission.Application.Common.Interfaces;
using MacroMission.Application.Common.Messaging;
using MacroMission.Application.Social.Commands.CreatePost;
using MacroMission.Application.Social.Results;
using MacroMission.Domain.Common;
using MacroMission.Domain.Social;

namespace MacroMission.Application.Social.Queries.GetPostById;

internal sealed class GetPostByIdQueryHandler(
    IPostRepository postRepository,
    IFollowRepository followRepository) : IQueryHandler<GetPostByIdQuery, PostResult>
{
    public async Task<Result<PostResult>> Handle(
        GetPostByIdQuery query,
        CancellationToken cancellationToken)
    {
        Post? post = await postRepository.GetByIdAsync(query.PostId, cancellationToken);

        if (post is null)
            return Result<PostResult>.Failure(Error.NotFound("Post.NotFound", "Post not found."));

        // Enforce visibility rules.
        if (post.AuthorId != query.RequesterId)
        {
            if (post.Visibility == PostVisibility.Private)
                return Result<PostResult>.Failure(Error.Forbidden("Post.Forbidden", "This post is private."));

            if (post.Visibility == PostVisibility.FollowersOnly)
            {
                bool isFollowing = await followRepository.IsFollowingAsync(
                    query.RequesterId, post.AuthorId, cancellationToken);

                if (!isFollowing)
                    return Result<PostResult>.Failure(Error.Forbidden("Post.Forbidden", "This post is for followers only."));
            }
        }

        return Result<PostResult>.Success(CreatePostCommandHandler.ToResult(post));
    }
}
