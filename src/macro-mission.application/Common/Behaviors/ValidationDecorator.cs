using FluentValidation;
using FluentValidation.Results;
using MacroMission.Application.Common.Messaging;
using MacroMission.Domain.Common;

namespace MacroMission.Application.Common.Behaviors;

public static class ValidationDecorator
{
    /// <summary>Wraps ICommandHandler&lt;TCommand, TResponse&gt; — commands that return a value.</summary>
    public sealed class CommandHandler<TCommand, TResponse>(
        ICommandHandler<TCommand, TResponse> inner,
        IEnumerable<IValidator<TCommand>> validators)
        : ICommandHandler<TCommand, TResponse>
        where TCommand : ICommand<TResponse>
    {
        public async Task<Result<TResponse>> Handle(TCommand command, CancellationToken cancellationToken)
        {
            ValidationError? validationError = Validate(command, validators);
            if (validationError is not null)
                return Result<TResponse>.Failure(validationError);

            return await inner.Handle(command, cancellationToken);
        }
    }

    /// <summary>Wraps ICommandHandler&lt;TCommand&gt; — void commands.</summary>
    public sealed class CommandBaseHandler<TCommand>(
        ICommandHandler<TCommand> inner,
        IEnumerable<IValidator<TCommand>> validators)
        : ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        public async Task<Result> Handle(TCommand command, CancellationToken cancellationToken)
        {
            ValidationError? validationError = Validate(command, validators);
            if (validationError is not null)
                return Result.Failure(validationError);

            return await inner.Handle(command, cancellationToken);
        }
    }

    private static ValidationError? Validate<TCommand>(
        TCommand command,
        IEnumerable<IValidator<TCommand>> validators)
    {
        List<ValidationFailure> failures = validators
            .Select(v => v.Validate(command))
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .ToList();

        if (failures.Count == 0)
            return null;

        Error[] errors = failures
            .Select(f => Error.Validation(f.PropertyName, f.ErrorMessage))
            .ToArray();

        return new ValidationError(errors);
    }
}
