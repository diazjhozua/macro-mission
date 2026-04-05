using System.Security.Claims;
using MacroMission.Api.Extensions;
using MacroMission.Api.Infrastructure;
using MacroMission.Application.Common.Messaging;
using MacroMission.Application.Social.Commands.UpdatePost;
using MacroMission.Application.Social.Results;
using MacroMission.Contracts.Social;
using MacroMission.Domain.Common;
using MacroMission.Domain.Social;
using MongoDB.Bson;

namespace MacroMission.Api.Endpoints.Social;

internal sealed class UpdatePost : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("api/v1/posts/{id}", async (
            string id,
            UpdatePostRequest request,
            ClaimsPrincipal user,
            ICommandHandler<UpdatePostCommand, PostResult> handler,
            CancellationToken cancellationToken) =>
        {
            if (!ObjectId.TryParse(id, out ObjectId postId))
                return Results.BadRequest(new { message = "Invalid post ID format." });

            if (!Enum.TryParse(request.Visibility, ignoreCase: true, out PostVisibility visibility))
                return Results.BadRequest(new { message = $"Invalid visibility. Valid values: {string.Join(", ", Enum.GetNames<PostVisibility>())}" });

            Result<PostResult> result = await handler.Handle(
                new UpdatePostCommand(postId, user.GetUserId(), request.Caption, visibility),
                cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Social)
        .WithSummary("Update a post's caption or visibility.")
        .Produces<PostResult>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status403Forbidden)
        .RequireAuthorization();
    }
}
