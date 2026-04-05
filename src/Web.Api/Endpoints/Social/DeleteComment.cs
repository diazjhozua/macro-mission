using System.Security.Claims;
using MacroMission.Api.Extensions;
using MacroMission.Api.Infrastructure;
using MacroMission.Application.Common.Messaging;
using MacroMission.Application.Social.Commands.DeleteComment;
using MacroMission.Domain.Common;
using MongoDB.Bson;

namespace MacroMission.Api.Endpoints.Social;

internal sealed class DeleteComment : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("api/v1/posts/{id}/comments/{commentId}", async (
            string id,
            string commentId,
            ClaimsPrincipal user,
            ICommandHandler<DeleteCommentCommand> handler,
            CancellationToken cancellationToken) =>
        {
            if (!ObjectId.TryParse(commentId, out ObjectId parsedCommentId))
                return Results.BadRequest(new { message = "Invalid comment ID format." });

            Result result = await handler.Handle(
                new DeleteCommentCommand(parsedCommentId, user.GetUserId()), cancellationToken);

            return result.Match(() => Results.NoContent(), CustomResults.Problem);
        })
        .WithTags(Tags.Social)
        .WithSummary("Delete a comment.")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status403Forbidden)
        .RequireAuthorization();
    }
}
