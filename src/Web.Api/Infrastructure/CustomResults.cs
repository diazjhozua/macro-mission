using MacroMission.Domain.Common;
using Microsoft.AspNetCore.Mvc;

namespace MacroMission.Api.Infrastructure;

internal static class CustomResults
{
    internal static IResult Problem(Error error)
    {
        if (error is ValidationError validationError)
            return ValidationProblem(validationError);

        int statusCode = error.Type switch
        {
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };

        return Results.Problem(statusCode: statusCode, title: error.Description);
    }

    private static IResult ValidationProblem(ValidationError validationError)
    {
        Dictionary<string, string[]> errors = validationError.Errors
            .GroupBy(e => e.Code)
            .ToDictionary(g => g.Key, g => g.Select(e => e.Description).ToArray());

        return Results.ValidationProblem(errors);
    }
}
