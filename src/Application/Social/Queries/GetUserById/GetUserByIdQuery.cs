using MacroMission.Application.Common.Messaging;
using MacroMission.Application.Social.Results;
using MongoDB.Bson;

namespace MacroMission.Application.Social.Queries.GetUserById;

public sealed record GetUserByIdQuery(ObjectId UserId) : IQuery<UserSummaryResult>;
