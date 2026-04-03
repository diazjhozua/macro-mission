using ErrorOr;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MacroMission.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public abstract class ApiController : ControllerBase
{
    protected IActionResult Problem(List<Error> errors)
    {
        if (errors.Count == 0)
            return Problem();

        // All validation errors → 400 with each error listed.
        if (errors.All(e => e.Type == ErrorType.Validation))
            return ValidationProblem(errors);

        return Problem(errors[0]);
    }

    private IActionResult Problem(Error error)
    {
        int statusCode = error.Type switch
        {
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };

        return Problem(statusCode: statusCode, title: error.Description);
    }

    private IActionResult ValidationProblem(List<Error> errors)
    {
        ModelStateDictionary modelState = new();
        foreach (Error error in errors)
            modelState.AddModelError(error.Code, error.Description);

        return ValidationProblem(modelState);
    }
}
