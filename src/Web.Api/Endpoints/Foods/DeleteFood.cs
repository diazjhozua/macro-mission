using System.Security.Claims;
using MacroMission.Api.Extensions;
using MacroMission.Api.Infrastructure;
using MacroMission.Application.Common.Messaging;
using MacroMission.Application.Foods.Commands.DeleteFood;
using MacroMission.Domain.Common;
using MongoDB.Bson;

namespace MacroMission.Api.Endpoints.Foods;

internal sealed class DeleteFood : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("api/v1/foods/{id}", async (
            string id,
            ClaimsPrincipal user,
            ICommandHandler<DeleteFoodCommand> handler,
            CancellationToken cancellationToken) =>
        {
            if (!ObjectId.TryParse(id, out ObjectId foodId))
                return Results.BadRequest(new { message = "Invalid food ID format." });

            Result result = await handler.Handle(
                new DeleteFoodCommand(foodId, user.GetUserId()), cancellationToken);

            return result.Match(
                () => Results.NoContent(),
                CustomResults.Problem);
        })
        .WithTags(Tags.Foods)
        .WithSummary("Delete your custom food.")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status403Forbidden)
        .RequireAuthorization();
    }
}
