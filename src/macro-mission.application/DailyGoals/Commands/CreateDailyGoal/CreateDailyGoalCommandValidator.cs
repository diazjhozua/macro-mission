using FluentValidation;

namespace MacroMission.Application.DailyGoals.Commands.CreateDailyGoal;

public sealed class CreateDailyGoalCommandValidator : AbstractValidator<CreateDailyGoalCommand>
{
    public CreateDailyGoalCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Calories)
            .GreaterThan(0);

        RuleFor(x => x.Protein)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.Carbs)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.Fat)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.Fiber)
            .GreaterThanOrEqualTo(0);
    }
}
