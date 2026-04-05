using System.Security.Claims;
using MacroMission.Api.Extensions;
using MacroMission.Api.Infrastructure;
using MacroMission.Application.Common.Messaging;
using MacroMission.Application.Social.Queries.GetUserPosts;
using MacroMission.Application.Social.Results;
using MacroMission.Domain.Common;
using MongoDB.Bson;

namespace MacroMission.Api.Endpoints.Social;

internal sealed class GetUserPosts : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v1/users/{userId}/posts", async (
            string userId,
            ClaimsPrincipal user,
            IQueryHandler<GetUserPostsQuery, List<PostResult>> handler,
            CancellationToken cancellationToken,
            int page = 1,
            int pageSize = 20) =>
        {
            if (!ObjectId.TryParse(userId, out ObjectId authorId))
                return Results.BadRequest(new { message = "Invalid user ID format." });

            Result<List<PostResult>> result = await handler.Handle(
                new GetUserPostsQuery(authorId, user.GetUserId(), page, pageSize),
                cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Social)
        .WithSummary("Get a user's posts.")
        .Produces<List<PostResult>>(StatusCodes.Status200OK)
        .RequireAuthorization();
    }
}
