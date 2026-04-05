using MacroMission.Application.Common.Messaging;
using MacroMission.Application.Social.Results;
using MongoDB.Bson;

namespace MacroMission.Application.Social.Queries.GetFeed;

public sealed record GetFeedQuery(
    ObjectId UserId,
    int Page,
    int PageSize) : IQuery<List<PostResult>>;
