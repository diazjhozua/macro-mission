using MacroMission.Api.Infrastructure;
using MacroMission.Application.Common.Messaging;
using MacroMission.Application.Social.Queries.GetFollowers;
using MacroMission.Application.Social.Results;
using MacroMission.Domain.Common;
using MongoDB.Bson;

namespace MacroMission.Api.Endpoints.Social;

internal sealed class GetFollowers : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v1/users/{userId}/followers", async (
            string userId,
            IQueryHandler<GetFollowersQuery, List<UserSummaryResult>> handler,
            CancellationToken cancellationToken) =>
        {
            if (!ObjectId.TryParse(userId, out ObjectId parsedUserId))
                return Results.BadRequest(new { message = "Invalid user ID format." });

            Result<List<UserSummaryResult>> result = await handler.Handle(
                new GetFollowersQuery(parsedUserId), cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Social)
        .WithSummary("Get the list of users who follow this user.")
        .Produces<List<UserSummaryResult>>(StatusCodes.Status200OK)
        .RequireAuthorization();
    }
}
