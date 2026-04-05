using MacroMission.Application.Common.Interfaces;
using MacroMission.Application.Common.Messaging;
using MacroMission.Domain.Common;
using MacroMission.Domain.DailyGoals;

namespace MacroMission.Application.DailyGoals.Commands.DeleteDailyGoal;

internal sealed class DeleteDailyGoalCommandHandler(
    IDailyGoalRepository dailyGoalRepository) : ICommandHandler<DeleteDailyGoalCommand>
{
    public async Task<Result> Handle(DeleteDailyGoalCommand command, CancellationToken cancellationToken)
    {
        DailyGoal? goal = await dailyGoalRepository.GetByIdAsync(command.GoalId, cancellationToken);

        if (goal is null)
            return Result.Failure(Error.NotFound("DailyGoal.NotFound", "Daily goal not found."));

        if (goal.UserId != command.UserId)
            return Result.Failure(Error.Forbidden("DailyGoal.Forbidden", "You do not have access to this goal."));

        await dailyGoalRepository.DeleteAsync(command.GoalId, cancellationToken);

        return Result.Success();
    }
}
