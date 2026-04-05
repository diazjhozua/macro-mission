using System.Security.Claims;
using MacroMission.Api.Extensions;
using MacroMission.Api.Infrastructure;
using MacroMission.Application.Common.Messaging;
using MacroMission.Application.Social.Commands.DeletePost;
using MacroMission.Domain.Common;
using MongoDB.Bson;

namespace MacroMission.Api.Endpoints.Social;

internal sealed class DeletePost : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("api/v1/posts/{id}", async (
            string id,
            ClaimsPrincipal user,
            ICommandHandler<DeletePostCommand> handler,
            CancellationToken cancellationToken) =>
        {
            if (!ObjectId.TryParse(id, out ObjectId postId))
                return Results.BadRequest(new { message = "Invalid post ID format." });

            Result result = await handler.Handle(
                new DeletePostCommand(postId, user.GetUserId()), cancellationToken);

            return result.Match(() => Results.NoContent(), CustomResults.Problem);
        })
        .WithTags(Tags.Social)
        .WithSummary("Delete a post.")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status403Forbidden)
        .RequireAuthorization();
    }
}
