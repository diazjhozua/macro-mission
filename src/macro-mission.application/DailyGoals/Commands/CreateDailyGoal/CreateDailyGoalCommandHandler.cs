using ErrorOr;
using MacroMission.Application.Common.Interfaces;
using MacroMission.Application.DailyGoals.Results;
using MacroMission.Domain.DailyGoals;
using MediatR;

namespace MacroMission.Application.DailyGoals.Commands.CreateDailyGoal;

public sealed class CreateDailyGoalCommandHandler(
    IDailyGoalRepository dailyGoalRepository)
    : IRequestHandler<CreateDailyGoalCommand, ErrorOr<DailyGoalResult>>
{
    public async Task<ErrorOr<DailyGoalResult>> Handle(
        CreateDailyGoalCommand command,
        CancellationToken cancellationToken)
    {
        bool hasActiveGoal = await dailyGoalRepository
            .GetActiveByUserIdAsync(command.UserId, cancellationToken) is not null;

        DailyGoal goal = new()
        {
            UserId = command.UserId,
            Name = command.Name,
            // First goal created is automatically set as active.
            IsActive = !hasActiveGoal,
            Calories = command.Calories,
            Protein = command.Protein,
            Carbs = command.Carbs,
            Fat = command.Fat,
            Fiber = command.Fiber
        };

        await dailyGoalRepository.CreateAsync(goal, cancellationToken);

        return ToResult(goal);
    }

    internal static DailyGoalResult ToResult(DailyGoal goal) => new(
        goal.Id.ToString(),
        goal.Name,
        goal.IsActive,
        goal.Calories,
        goal.Protein,
        goal.Carbs,
        goal.Fat,
        goal.Fiber,
        goal.CreatedAt,
        goal.UpdatedAt);
}
