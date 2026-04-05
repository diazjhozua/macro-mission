using MacroMission.Application.Common.Interfaces;
using MacroMission.Application.Common.Messaging;
using MacroMission.Application.DailyGoals.Commands.CreateDailyGoal;
using MacroMission.Application.DailyGoals.Results;
using MacroMission.Domain.Common;
using MacroMission.Domain.DailyGoals;

namespace MacroMission.Application.DailyGoals.Queries.GetAllDailyGoals;

internal sealed class GetAllDailyGoalsQueryHandler(
    IDailyGoalRepository dailyGoalRepository)
    : IQueryHandler<GetAllDailyGoalsQuery, List<DailyGoalResult>>
{
    public async Task<Result<List<DailyGoalResult>>> Handle(
        GetAllDailyGoalsQuery query,
        CancellationToken cancellationToken)
    {
        List<DailyGoal> goals = await dailyGoalRepository
            .GetAllByUserIdAsync(query.UserId, cancellationToken);

        return Result<List<DailyGoalResult>>.Success(
            goals.Select(CreateDailyGoalCommandHandler.ToResult).ToList());
    }
}
