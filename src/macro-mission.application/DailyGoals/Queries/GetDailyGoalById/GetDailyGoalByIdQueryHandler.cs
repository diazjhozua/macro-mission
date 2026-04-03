using ErrorOr;
using MacroMission.Application.Common.Interfaces;
using MacroMission.Application.DailyGoals.Commands.CreateDailyGoal;
using MacroMission.Application.DailyGoals.Results;
using MacroMission.Domain.DailyGoals;
using MediatR;

namespace MacroMission.Application.DailyGoals.Queries.GetDailyGoalById;

public sealed class GetDailyGoalByIdQueryHandler(
    IDailyGoalRepository dailyGoalRepository)
    : IRequestHandler<GetDailyGoalByIdQuery, ErrorOr<DailyGoalResult>>
{
    public async Task<ErrorOr<DailyGoalResult>> Handle(
        GetDailyGoalByIdQuery query,
        CancellationToken cancellationToken)
    {
        DailyGoal? goal = await dailyGoalRepository.GetByIdAsync(query.GoalId, cancellationToken);

        if (goal is null)
            return Error.NotFound("DailyGoal.NotFound", "Daily goal not found.");

        if (goal.UserId != query.UserId)
            return Error.Forbidden("DailyGoal.Forbidden", "You do not have access to this goal.");

        return CreateDailyGoalCommandHandler.ToResult(goal);
    }
}
