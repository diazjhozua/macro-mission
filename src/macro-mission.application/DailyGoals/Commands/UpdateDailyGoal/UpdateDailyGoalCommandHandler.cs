using ErrorOr;
using MacroMission.Application.Common.Interfaces;
using MacroMission.Application.DailyGoals.Commands.CreateDailyGoal;
using MacroMission.Application.DailyGoals.Results;
using MacroMission.Domain.DailyGoals;
using MediatR;

namespace MacroMission.Application.DailyGoals.Commands.UpdateDailyGoal;

public sealed class UpdateDailyGoalCommandHandler(
    IDailyGoalRepository dailyGoalRepository)
    : IRequestHandler<UpdateDailyGoalCommand, ErrorOr<DailyGoalResult>>
{
    public async Task<ErrorOr<DailyGoalResult>> Handle(
        UpdateDailyGoalCommand command,
        CancellationToken cancellationToken)
    {
        DailyGoal? goal = await dailyGoalRepository.GetByIdAsync(command.GoalId, cancellationToken);

        if (goal is null)
            return Error.NotFound("DailyGoal.NotFound", "Daily goal not found.");

        // Prevent users from modifying another user's goals.
        if (goal.UserId != command.UserId)
            return Error.Forbidden("DailyGoal.Forbidden", "You do not have access to this goal.");

        goal.Name = command.Name;
        goal.Calories = command.Calories;
        goal.Protein = command.Protein;
        goal.Carbs = command.Carbs;
        goal.Fat = command.Fat;
        goal.Fiber = command.Fiber;
        goal.UpdatedAt = DateTime.UtcNow;

        await dailyGoalRepository.UpdateAsync(goal, cancellationToken);

        return CreateDailyGoalCommandHandler.ToResult(goal);
    }
}
