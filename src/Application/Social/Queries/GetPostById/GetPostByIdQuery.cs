using MacroMission.Application.Common.Messaging;
using MacroMission.Application.Social.Results;
using MongoDB.Bson;

namespace MacroMission.Application.Social.Queries.GetPostById;

public sealed record GetPostByIdQuery(ObjectId PostId, ObjectId RequesterId) : IQuery<PostResult>;
