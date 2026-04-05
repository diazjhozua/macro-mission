using MacroMission.Domain.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MacroMission.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public abstract class ApiController : ControllerBase
{
    protected IActionResult Problem(Error error)
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

        return Problem(statusCode: statusCode, title: error.Description);
    }

    private IActionResult ValidationProblem(ValidationError validationError)
    {
        ModelStateDictionary modelState = new();
        foreach (Error error in validationError.Errors)
            modelState.AddModelError(error.Code, error.Description);

        return ValidationProblem(modelState);
    }
}
