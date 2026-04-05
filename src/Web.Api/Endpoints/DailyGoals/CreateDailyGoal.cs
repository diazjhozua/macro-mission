using System.Security.Claims;
using MacroMission.Api.Extensions;
using MacroMission.Api.Infrastructure;
using MacroMission.Application.Common.Messaging;
using MacroMission.Application.DailyGoals.Commands.CreateDailyGoal;
using MacroMission.Application.DailyGoals.Results;
using MacroMission.Contracts.DailyGoals;
using MacroMission.Domain.Common;

namespace MacroMission.Api.Endpoints.DailyGoals;

internal sealed class CreateDailyGoal : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/v1/dailygoals", async (
            CreateDailyGoalRequest request,
            ClaimsPrincipal user,
            ICommandHandler<CreateDailyGoalCommand, DailyGoalResult> handler,
            CancellationToken cancellationToken) =>
        {
            CreateDailyGoalCommand command = new(
                user.GetUserId(),
                request.Name,
                request.Calories,
                request.Protein,
                request.Carbs,
                request.Fat,
                request.Fiber);

            Result<DailyGoalResult> result = await handler.Handle(command, cancellationToken);

            return result.Match(
                goal => Results.Created($"api/v1/dailygoals/{goal.Id}", goal),
                CustomResults.Problem);
        })
        .WithTags(Tags.DailyGoals)
        .WithSummary("Create a new daily goal.")
        .Produces<DailyGoalResult>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .RequireAuthorization();
    }
}
