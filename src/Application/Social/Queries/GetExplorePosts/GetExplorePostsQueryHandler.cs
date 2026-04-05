using MacroMission.Application.Common.Interfaces;
using MacroMission.Application.Common.Messaging;
using MacroMission.Application.Social.Commands.CreatePost;
using MacroMission.Application.Social.Results;
using MacroMission.Domain.Common;
using MacroMission.Domain.Social;

namespace MacroMission.Application.Social.Queries.GetExplorePosts;

internal sealed class GetExplorePostsQueryHandler(
    IPostRepository postRepository) : IQueryHandler<GetExplorePostsQuery, List<PostResult>>
{
    public async Task<Result<List<PostResult>>> Handle(
        GetExplorePostsQuery query,
        CancellationToken cancellationToken)
    {
        List<Post> posts = await postRepository.GetPublicPostsAsync(
            query.Page, query.PageSize, cancellationToken);

        return Result<List<PostResult>>.Success(
            posts.Select(CreatePostCommandHandler.ToResult).ToList());
    }
}
