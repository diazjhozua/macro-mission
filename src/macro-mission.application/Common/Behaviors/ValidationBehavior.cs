using ErrorOr;
using FluentValidation;
using MediatR;

namespace MacroMission.Application.Common.Behaviors;

/// <summary>
/// Runs all registered FluentValidation validators before the handler executes.
/// Requests with no validators are passed through with zero overhead.
/// </summary>
public sealed class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : IErrorOr
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!validators.Any())
            return await next(cancellationToken);

        var errors = validators
            .Select(v => v.Validate(request))
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .Select(f => Error.Validation(f.PropertyName, f.ErrorMessage))
            .ToList();

        if (errors.Count > 0)
            return (TResponse)(dynamic)errors;

        return await next(cancellationToken);
    }
}
