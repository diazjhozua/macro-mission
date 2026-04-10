using System.Security.Claims;
using MacroMission.Api.Extensions;
using MacroMission.Api.Infrastructure;
using MacroMission.Application.Common.Messaging;
using MacroMission.Application.Meals.Results;
using MacroMission.Application.Social.Queries.GetPostMeal;
using MacroMission.Domain.Common;
using MongoDB.Bson;

namespace MacroMission.Api.Endpoints.Social;

internal sealed class GetPostMeal : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v1/posts/{id}/meal", async (
            string id,
            ClaimsPrincipal user,
            IQueryHandler<GetPostMealQuery, MealResult> handler,
            CancellationToken cancellationToken) =>
        {
            if (!ObjectId.TryParse(id, out ObjectId postId))
                return Results.BadRequest(new { message = "Invalid post ID format." });

            Result<MealResult> result = await handler.Handle(
                new GetPostMealQuery(postId, user.GetUserId()), cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Social)
        .WithSummary("Get the meal linked to a post.")
        .Produces<MealResult>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status403Forbidden)
        .Produces(StatusCodes.Status404NotFound)
        .RequireAuthorization();
    }
}
