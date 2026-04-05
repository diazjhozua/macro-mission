using MacroMission.Application.Common.Messaging;
using MacroMission.Application.Social.Results;
using MongoDB.Bson;

namespace MacroMission.Application.Social.Queries.GetComments;

public sealed record GetCommentsQuery(
    ObjectId PostId,
    int Page,
    int PageSize) : IQuery<List<CommentResult>>;
