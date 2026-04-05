using MacroMission.Application.Common.Messaging;
using MacroMission.Application.Social.Results;

namespace MacroMission.Application.Social.Queries.GetExplorePosts;

public sealed record GetExplorePostsQuery(int Page, int PageSize) : IQuery<List<PostResult>>;
