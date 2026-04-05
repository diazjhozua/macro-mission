using System.Security.Claims;
using MacroMission.Api.Extensions;
using MacroMission.Api.Infrastructure;
using MacroMission.Application.Common.Messaging;
using MacroMission.Application.Social.Commands.UnlikePost;
using MacroMission.Domain.Common;
using MongoDB.Bson;

namespace MacroMission.Api.Endpoints.Social;

internal sealed class UnlikePost : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("api/v1/posts/{id}/likes", async (
            string id,
            ClaimsPrincipal user,
            ICommandHandler<UnlikePostCommand> handler,
            CancellationToken cancellationToken) =>
        {
            if (!ObjectId.TryParse(id, out ObjectId postId))
                return Results.BadRequest(new { message = "Invalid post ID format." });

            Result result = await handler.Handle(
                new UnlikePostCommand(user.GetUserId(), postId), cancellationToken);

            return result.Match(() => Results.NoContent(), CustomResults.Problem);
        })
        .WithTags(Tags.Social)
        .WithSummary("Unlike a post.")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound)
        .RequireAuthorization();
    }
}
