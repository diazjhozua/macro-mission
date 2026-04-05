using System.Security.Claims;
using MacroMission.Api.Extensions;
using MacroMission.Api.Infrastructure;
using MacroMission.Application.Common.Messaging;
using MacroMission.Application.Social.Commands.LikePost;
using MacroMission.Domain.Common;
using MongoDB.Bson;

namespace MacroMission.Api.Endpoints.Social;

internal sealed class LikePost : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/v1/posts/{id}/likes", async (
            string id,
            ClaimsPrincipal user,
            ICommandHandler<LikePostCommand> handler,
            CancellationToken cancellationToken) =>
        {
            if (!ObjectId.TryParse(id, out ObjectId postId))
                return Results.BadRequest(new { message = "Invalid post ID format." });

            Result result = await handler.Handle(
                new LikePostCommand(user.GetUserId(), postId), cancellationToken);

            return result.Match(() => Results.Ok(), CustomResults.Problem);
        })
        .WithTags(Tags.Social)
        .WithSummary("Like a post.")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status409Conflict)
        .RequireAuthorization();
    }
}
