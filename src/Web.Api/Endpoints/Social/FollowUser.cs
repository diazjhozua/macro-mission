using System.Security.Claims;
using MacroMission.Api.Extensions;
using MacroMission.Api.Infrastructure;
using MacroMission.Application.Common.Messaging;
using MacroMission.Application.Social.Commands.Follow;
using MacroMission.Domain.Common;
using MongoDB.Bson;

namespace MacroMission.Api.Endpoints.Social;

internal sealed class FollowUser : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/v1/users/{userId}/follow", async (
            string userId,
            ClaimsPrincipal user,
            ICommandHandler<FollowCommand> handler,
            CancellationToken cancellationToken) =>
        {
            if (!ObjectId.TryParse(userId, out ObjectId followingId))
                return Results.BadRequest(new { message = "Invalid user ID format." });

            Result result = await handler.Handle(
                new FollowCommand(user.GetUserId(), followingId), cancellationToken);

            return result.Match(() => Results.Ok(), CustomResults.Problem);
        })
        .WithTags(Tags.Social)
        .WithSummary("Follow a user.")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status409Conflict)
        .RequireAuthorization();
    }
}
