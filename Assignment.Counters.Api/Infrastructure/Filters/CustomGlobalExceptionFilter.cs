using Assignment.Counters.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Assignment.Counters.Api.Infrastructure.Filters;

public sealed class CustomGlobalExceptionFilter : IExceptionFilter
{
    private const string MessageText = "Unknown error";
    
    private readonly ILogger<CustomGlobalExceptionFilter> _logger;

    public CustomGlobalExceptionFilter(ILogger<CustomGlobalExceptionFilter> logger)
    {
        _logger = logger;
    }
    
    public void OnException(ExceptionContext context)
    {
        _logger.LogError(context.Exception, MessageText);

        var statusCode = SelectStatusCode(context);

        context.Result = new ObjectResult(new
        {
            Error = "An unexpected error has occurred."
        })
        {
            StatusCode = statusCode
        };
        
        context.ExceptionHandled = true;
    }

    private int SelectStatusCode(ExceptionContext context)
    {
        return context.Exception.GetType() == typeof(EntryNotFoundException<>) ?
            StatusCodes.Status404NotFound :
            StatusCodes.Status500InternalServerError;
    }
}