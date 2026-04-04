using ErrorOr;
using MacroMission.Application.Common.Interfaces;
using MacroMission.Domain.DailyGoals;
using MediatR;

namespace MacroMission.Application.DailyGoals.Commands.DeleteDailyGoal;

public sealed class DeleteDailyGoalCommandHandler(
    IDailyGoalRepository dailyGoalRepository)
    : IRequestHandler<DeleteDailyGoalCommand, ErrorOr<Deleted>>
{
    public async Task<ErrorOr<Deleted>> Handle(
        DeleteDailyGoalCommand command,
        CancellationToken cancellationToken)
    {
        DailyGoal? goal = await dailyGoalRepository.GetByIdAsync(command.GoalId, cancellationToken);

        if (goal is null)
            return Error.NotFound("DailyGoal.NotFound", "Daily goal not found.");

        if (goal.UserId != command.UserId)
            return Error.Forbidden("DailyGoal.Forbidden", "You do not have access to this goal.");

        await dailyGoalRepository.DeleteAsync(command.GoalId, cancellationToken);

        return Result.Deleted;
    }
}
