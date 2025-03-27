using Assignment.Counters.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Assignment.Counters.Api.Controllers;

public abstract class BaseController : ControllerBase
{
    protected IActionResult SpecifyErrorResponse(Exception exception)
    {
        if (exception.GetType().IsGenericType && exception.GetType().GetGenericTypeDefinition() == typeof(EntryNotFoundException<>))
            return NotFound();
        
        return Problem(exception.Message, statusCode: StatusCodes.Status500InternalServerError);
    }
}