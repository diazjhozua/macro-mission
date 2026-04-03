using ErrorOr;
using MacroMission.Application.Common.Interfaces;
using MacroMission.Application.DailyGoals.Commands.CreateDailyGoal;
using MacroMission.Application.DailyGoals.Results;
using MacroMission.Domain.DailyGoals;
using MediatR;

namespace MacroMission.Application.DailyGoals.Queries.GetAllDailyGoals;

public sealed class GetAllDailyGoalsQueryHandler(
    IDailyGoalRepository dailyGoalRepository)
    : IRequestHandler<GetAllDailyGoalsQuery, ErrorOr<List<DailyGoalResult>>>
{
    public async Task<ErrorOr<List<DailyGoalResult>>> Handle(
        GetAllDailyGoalsQuery query,
        CancellationToken cancellationToken)
    {
        List<DailyGoal> goals = await dailyGoalRepository
            .GetAllByUserIdAsync(query.UserId, cancellationToken);

        return goals.Select(CreateDailyGoalCommandHandler.ToResult).ToList();
    }
}
