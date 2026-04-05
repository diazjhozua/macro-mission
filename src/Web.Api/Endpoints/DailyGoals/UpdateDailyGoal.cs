using System.Security.Claims;
using MacroMission.Api.Extensions;
using MacroMission.Api.Infrastructure;
using MacroMission.Application.Common.Messaging;
using MacroMission.Application.DailyGoals.Commands.UpdateDailyGoal;
using MacroMission.Application.DailyGoals.Results;
using MacroMission.Contracts.DailyGoals;
using MacroMission.Domain.Common;
using MongoDB.Bson;

namespace MacroMission.Api.Endpoints.DailyGoals;

internal sealed class UpdateDailyGoal : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("api/v1/dailygoals/{id}", async (
            string id,
            UpdateDailyGoalRequest request,
            ClaimsPrincipal user,
            ICommandHandler<UpdateDailyGoalCommand, DailyGoalResult> handler,
            CancellationToken cancellationToken) =>
        {
            if (!ObjectId.TryParse(id, out ObjectId goalId))
                return Results.BadRequest(new { message = "Invalid goal ID format." });

            UpdateDailyGoalCommand command = new(
                goalId,
                user.GetUserId(),
                request.Name,
                request.IsActive,
                request.Calories,
                request.Protein,
                request.Carbs,
                request.Fat,
                request.Fiber);

            Result<DailyGoalResult> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.DailyGoals)
        .WithSummary("Update a daily goal. Set IsActive to true to make it the current active goal.")
        .Produces<DailyGoalResult>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status403Forbidden)
        .Produces(StatusCodes.Status400BadRequest)
        .RequireAuthorization();
    }
}
