using FluentValidation;

namespace MacroMission.Application.Social.Commands.AddComment;

public sealed class AddCommentCommandValidator : AbstractValidator<AddCommentCommand>
{
    public AddCommentCommandValidator()
    {
        RuleFor(x => x.Text)
            .NotEmpty()
            .MaximumLength(500);
    }
}
