using System.Security.Claims;
using MacroMission.Api.Extensions;
using MacroMission.Api.Infrastructure;
using MacroMission.Application.Common.Messaging;
using MacroMission.Application.DailyGoals.Commands.DeleteDailyGoal;
using MacroMission.Domain.Common;
using MongoDB.Bson;

namespace MacroMission.Api.Endpoints.DailyGoals;

internal sealed class DeleteDailyGoal : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("api/v1/dailygoals/{id}", async (
            string id,
            ClaimsPrincipal user,
            ICommandHandler<DeleteDailyGoalCommand> handler,
            CancellationToken cancellationToken) =>
        {
            if (!ObjectId.TryParse(id, out ObjectId goalId))
                return Results.BadRequest(new { message = "Invalid goal ID format." });

            Result result = await handler.Handle(
                new DeleteDailyGoalCommand(goalId, user.GetUserId()), cancellationToken);

            return result.Match(
                () => Results.NoContent(),
                CustomResults.Problem);
        })
        .WithTags(Tags.DailyGoals)
        .WithSummary("Delete a daily goal.")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status403Forbidden)
        .RequireAuthorization();
    }
}
