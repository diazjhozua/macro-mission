using MacroMission.Application.Common.Interfaces;
using MacroMission.Application.Common.Messaging;
using MacroMission.Application.Social.Commands.CreatePost;
using MacroMission.Application.Social.Results;
using MacroMission.Domain.Common;
using MacroMission.Domain.Social;
using MongoDB.Bson;

namespace MacroMission.Application.Social.Queries.GetFeed;

internal sealed class GetFeedQueryHandler(
    IPostRepository postRepository,
    IFollowRepository followRepository) : IQueryHandler<GetFeedQuery, List<PostResult>>
{
    public async Task<Result<List<PostResult>>> Handle(
        GetFeedQuery query,
        CancellationToken cancellationToken)
    {
        // Feed = posts from users the requester follows.
        // Include the requester's own posts too.
        List<ObjectId> followingIds = await followRepository
            .GetFollowingIdsAsync(query.UserId, cancellationToken);

        followingIds.Add(query.UserId);

        List<Post> posts = await postRepository.GetFeedAsync(
            followingIds, query.Page, query.PageSize, cancellationToken);

        return Result<List<PostResult>>.Success(
            posts.Select(CreatePostCommandHandler.ToResult).ToList());
    }
}
