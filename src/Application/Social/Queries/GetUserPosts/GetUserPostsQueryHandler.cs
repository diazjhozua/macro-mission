using MacroMission.Application.Common.Interfaces;
using MacroMission.Application.Common.Messaging;
using MacroMission.Application.Social.Commands.CreatePost;
using MacroMission.Application.Social.Results;
using MacroMission.Domain.Common;
using MacroMission.Domain.Social;
using MongoDB.Bson;

namespace MacroMission.Application.Social.Queries.GetUserPosts;

internal sealed class GetUserPostsQueryHandler(
    IPostRepository postRepository) : IQueryHandler<GetUserPostsQuery, List<PostResult>>
{
    public async Task<Result<List<PostResult>>> Handle(
        GetUserPostsQuery query,
        CancellationToken cancellationToken)
    {
        // Pass requesterId so the repository can filter by visibility.
        ObjectId? requesterId = query.RequesterId == query.AuthorId ? null : query.RequesterId;

        List<Post> posts = await postRepository.GetByAuthorAsync(
            query.AuthorId, requesterId, query.Page, query.PageSize, cancellationToken);

        return Result<List<PostResult>>.Success(
            posts.Select(CreatePostCommandHandler.ToResult).ToList());
    }
}
