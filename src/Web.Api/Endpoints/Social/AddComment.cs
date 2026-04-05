using System.Security.Claims;
using MacroMission.Api.Extensions;
using MacroMission.Api.Infrastructure;
using MacroMission.Application.Common.Messaging;
using MacroMission.Application.Social.Commands.AddComment;
using MacroMission.Application.Social.Results;
using MacroMission.Contracts.Social;
using MacroMission.Domain.Common;
using MongoDB.Bson;

namespace MacroMission.Api.Endpoints.Social;

internal sealed class AddComment : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/v1/posts/{id}/comments", async (
            string id,
            AddCommentRequest request,
            ClaimsPrincipal user,
            ICommandHandler<AddCommentCommand, CommentResult> handler,
            CancellationToken cancellationToken) =>
        {
            if (!ObjectId.TryParse(id, out ObjectId postId))
                return Results.BadRequest(new { message = "Invalid post ID format." });

            Result<CommentResult> result = await handler.Handle(
                new AddCommentCommand(postId, user.GetUserId(), request.Text),
                cancellationToken);

            return result.Match(
                comment => Results.Created($"api/v1/posts/{id}/comments/{comment.Id}", comment),
                CustomResults.Problem);
        })
        .WithTags(Tags.Social)
        .WithSummary("Add a comment to a post.")
        .Produces<CommentResult>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .RequireAuthorization();
    }
}
