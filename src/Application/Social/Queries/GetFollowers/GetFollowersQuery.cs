using MacroMission.Application.Common.Messaging;
using MacroMission.Application.Social.Results;
using MongoDB.Bson;

namespace MacroMission.Application.Social.Queries.GetFollowers;

public sealed record GetFollowersQuery(ObjectId UserId) : IQuery<List<UserSummaryResult>>;
