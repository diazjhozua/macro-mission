using MacroMission.Application.Common.Interfaces;
using MacroMission.Application.Common.Messaging;
using MacroMission.Application.DailyGoals.Commands.CreateDailyGoal;
using MacroMission.Application.DailyGoals.Results;
using MacroMission.Domain.Common;
using MacroMission.Domain.DailyGoals;

namespace MacroMission.Application.DailyGoals.Queries.GetDailyGoalById;

internal sealed class GetDailyGoalByIdQueryHandler(
    IDailyGoalRepository dailyGoalRepository)
    : IQueryHandler<GetDailyGoalByIdQuery, DailyGoalResult>
{
    public async Task<Result<DailyGoalResult>> Handle(
        GetDailyGoalByIdQuery query,
        CancellationToken cancellationToken)
    {
        DailyGoal? goal = await dailyGoalRepository.GetByIdAsync(query.GoalId, cancellationToken);

        if (goal is null)
            return Result<DailyGoalResult>.Failure(Error.NotFound("DailyGoal.NotFound", "Daily goal not found."));

        if (goal.UserId != query.UserId)
            return Result<DailyGoalResult>.Failure(Error.Forbidden("DailyGoal.Forbidden", "You do not have access to this goal."));

        return Result<DailyGoalResult>.Success(CreateDailyGoalCommandHandler.ToResult(goal));
    }
}
