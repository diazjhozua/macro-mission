using MacroMission.Api.Infrastructure;
using MacroMission.Application.Common.Messaging;
using MacroMission.Application.Social.Queries.GetUserById;
using MacroMission.Application.Social.Results;
using MacroMission.Domain.Common;
using MongoDB.Bson;

namespace MacroMission.Api.Endpoints.Social;

internal sealed class GetUserById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v1/users/{userId}", async (
            string userId,
            IQueryHandler<GetUserByIdQuery, UserSummaryResult> handler,
            CancellationToken cancellationToken) =>
        {
            if (!ObjectId.TryParse(userId, out ObjectId parsedUserId))
                return Results.BadRequest(new { message = "Invalid user ID format." });

            Result<UserSummaryResult> result = await handler.Handle(
                new GetUserByIdQuery(parsedUserId), cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Social)
        .WithSummary("Get a user's public profile by ID.")
        .Produces<UserSummaryResult>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound)
        .RequireAuthorization();
    }
}
