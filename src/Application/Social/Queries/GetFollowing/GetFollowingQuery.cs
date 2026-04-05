using MacroMission.Application.Common.Messaging;
using MacroMission.Application.Social.Results;
using MongoDB.Bson;

namespace MacroMission.Application.Social.Queries.GetFollowing;

public sealed record GetFollowingQuery(ObjectId UserId) : IQuery<List<UserSummaryResult>>;
