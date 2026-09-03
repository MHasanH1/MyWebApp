using Microsoft.AspNetCore.Diagnostics;

namespace MyWebApp.Handlers;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is ConflictException)
        {
            httpContext.Response.StatusCode = StatusCodes.Status409Conflict;

            _logger.LogWarning(
                exception,
                "Conflict occurred: {Method} {Path}",
                httpContext.Request.Method,
                httpContext.Request.Path
            );

            await httpContext.Response.WriteAsJsonAsync(new
            {
                status = 409,
                message = exception.Message
            }, cancellationToken);

            return true;   
        }
        else if (exception is BadHttpRequestException)
        {
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

            _logger.LogWarning(
                exception,
                "Invalid http request recived: {Method} {Path}",
                httpContext.Request.Method,
                httpContext.Request.Path
            );

            await httpContext.Response.WriteAsJsonAsync(new
            {
                status = 400,
                message = exception.Message
            }, cancellationToken);

            return true;            
        }

        _logger.LogError(
            exception,
            "Unhandled exception occurred: {Method} {Path}",
            httpContext.Request.Method,
            httpContext.Request.Path
        );

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

        await httpContext.Response.WriteAsJsonAsync(new
        {
            status = 500,
            message = "An unexpected error occurred"
        }, cancellationToken);

        return true;
    }
}