using MacroMission.Api.Infrastructure;
using MacroMission.Application.Common.Messaging;
using MacroMission.Application.Social.Queries.GetFollowing;
using MacroMission.Application.Social.Results;
using MacroMission.Domain.Common;
using MongoDB.Bson;

namespace MacroMission.Api.Endpoints.Social;

internal sealed class GetFollowing : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v1/users/{userId}/following", async (
            string userId,
            IQueryHandler<GetFollowingQuery, List<UserSummaryResult>> handler,
            CancellationToken cancellationToken) =>
        {
            if (!ObjectId.TryParse(userId, out ObjectId parsedUserId))
                return Results.BadRequest(new { message = "Invalid user ID format." });

            Result<List<UserSummaryResult>> result = await handler.Handle(
                new GetFollowingQuery(parsedUserId), cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Social)
        .WithSummary("Get the list of users this user is following.")
        .Produces<List<UserSummaryResult>>(StatusCodes.Status200OK)
        .RequireAuthorization();
    }
}
