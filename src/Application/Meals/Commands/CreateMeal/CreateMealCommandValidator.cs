using FluentValidation;

namespace MacroMission.Application.Meals.Commands.CreateMeal;

public sealed class CreateMealCommandValidator : AbstractValidator<CreateMealCommand>
{
    public CreateMealCommandValidator()
    {
        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("A meal must have at least one food item.");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.Grams)
                .GreaterThan(0).WithMessage("Grams must be greater than zero.");
        });
    }
}
