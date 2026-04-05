using FluentValidation;

namespace MacroMission.Application.Foods.Commands.CreateFood;

public sealed class CreateFoodCommandValidator : AbstractValidator<CreateFoodCommand>
{
    public CreateFoodCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Brand)
            .MaximumLength(100)
            .When(x => x.Brand is not null);

        RuleFor(x => x.Calories)
            .GreaterThanOrEqualTo(0);

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
