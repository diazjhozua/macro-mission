using System.Security.Claims;
using MacroMission.Api.Extensions;
using MacroMission.Api.Infrastructure;
using MacroMission.Application.Common.Messaging;
using MacroMission.Application.Social.Commands.Unfollow;
using MacroMission.Domain.Common;
using MongoDB.Bson;

namespace MacroMission.Api.Endpoints.Social;

internal sealed class UnfollowUser : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("api/v1/users/{userId}/follow", async (
            string userId,
            ClaimsPrincipal user,
            ICommandHandler<UnfollowCommand> handler,
            CancellationToken cancellationToken) =>
        {
            if (!ObjectId.TryParse(userId, out ObjectId followingId))
                return Results.BadRequest(new { message = "Invalid user ID format." });

            Result result = await handler.Handle(
                new UnfollowCommand(user.GetUserId(), followingId), cancellationToken);

            return result.Match(() => Results.NoContent(), CustomResults.Problem);
        })
        .WithTags(Tags.Social)
        .WithSummary("Unfollow a user.")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound)
        .RequireAuthorization();
    }
}
