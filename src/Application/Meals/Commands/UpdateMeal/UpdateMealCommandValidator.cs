using FluentValidation;

namespace MacroMission.Application.Meals.Commands.UpdateMeal;

public sealed class UpdateMealCommandValidator : AbstractValidator<UpdateMealCommand>
{
    public UpdateMealCommandValidator()
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
