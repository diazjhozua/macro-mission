using MacroMission.Application.Common.Interfaces;
using MacroMission.Application.Common.Messaging;
using MacroMission.Application.DailyGoals.Commands.CreateDailyGoal;
using MacroMission.Application.DailyGoals.Results;
using MacroMission.Domain.Common;
using MacroMission.Domain.DailyGoals;

namespace MacroMission.Application.DailyGoals.Commands.UpdateDailyGoal;

internal sealed class UpdateDailyGoalCommandHandler(
    IDailyGoalRepository dailyGoalRepository)
    : ICommandHandler<UpdateDailyGoalCommand, DailyGoalResult>
{
    public async Task<Result<DailyGoalResult>> Handle(
        UpdateDailyGoalCommand command,
        CancellationToken cancellationToken)
    {
        DailyGoal? goal = await dailyGoalRepository.GetByIdAsync(command.GoalId, cancellationToken);

        if (goal is null)
            return Result<DailyGoalResult>.Failure(Error.NotFound("DailyGoal.NotFound", "Daily goal not found."));

        if (goal.UserId != command.UserId)
            return Result<DailyGoalResult>.Failure(Error.Forbidden("DailyGoal.Forbidden", "You do not have access to this goal."));

        // Deactivate all other goals before activating this one.
        if (command.IsActive && !goal.IsActive)
            await dailyGoalRepository.DeactivateAllByUserIdAsync(command.UserId, cancellationToken);

        goal.Name = command.Name;
        goal.IsActive = command.IsActive;
        goal.Calories = command.Calories;
        goal.Protein = command.Protein;
        goal.Carbs = command.Carbs;
        goal.Fat = command.Fat;
        goal.Fiber = command.Fiber;
        goal.UpdatedAt = DateTime.UtcNow;

        await dailyGoalRepository.UpdateAsync(goal, cancellationToken);

        return Result<DailyGoalResult>.Success(CreateDailyGoalCommandHandler.ToResult(goal));
    }
}
