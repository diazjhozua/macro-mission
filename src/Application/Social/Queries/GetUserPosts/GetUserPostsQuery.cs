using MacroMission.Application.Common.Messaging;
using MacroMission.Application.Social.Results;
using MongoDB.Bson;

namespace MacroMission.Application.Social.Queries.GetUserPosts;

public sealed record GetUserPostsQuery(
    ObjectId AuthorId,
    ObjectId RequesterId,
    int Page,
    int PageSize) : IQuery<List<PostResult>>;
