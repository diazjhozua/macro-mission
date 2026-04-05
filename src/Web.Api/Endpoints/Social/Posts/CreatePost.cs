using System.Security.Claims;
using MacroMission.Api.Extensions;
using MacroMission.Api.Infrastructure;
using MacroMission.Application.Common.Messaging;
using MacroMission.Application.Social.Commands.CreatePost;
using MacroMission.Application.Social.Results;
using MacroMission.Contracts.Social;
using MacroMission.Domain.Common;
using MacroMission.Domain.Social;
using MongoDB.Bson;

namespace MacroMission.Api.Endpoints.Social;

internal sealed class CreatePost : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/v1/posts", async (
            CreatePostRequest request,
            ClaimsPrincipal user,
            ICommandHandler<CreatePostCommand, PostResult> handler,
            CancellationToken cancellationToken) =>
        {
            if (!ObjectId.TryParse(request.MealId, out ObjectId mealId))
                return Results.BadRequest(new { message = "Invalid meal ID format." });

            if (!Enum.TryParse(request.Visibility, ignoreCase: true, out PostVisibility visibility))
                return Results.BadRequest(new { message = $"Invalid visibility. Valid values: {string.Join(", ", Enum.GetNames<PostVisibility>())}" });

            Result<PostResult> result = await handler.Handle(
                new CreatePostCommand(user.GetUserId(), mealId, request.Caption, visibility),
                cancellationToken);

            return result.Match(
                post => Results.Created($"api/v1/posts/{post.Id}", post),
                CustomResults.Problem);
        })
        .WithTags(Tags.Social)
        .WithSummary("Share a meal as a post.")
        .Produces<PostResult>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .RequireAuthorization();
    }
}
