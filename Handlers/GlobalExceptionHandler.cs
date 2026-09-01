using Microsoft.AspNetCore.Diagnostics;

namespace MyWebApp.Handlers;

public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is BadHttpRequestException)
        {
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

             await httpContext.Response.WriteAsJsonAsync(new
            {
                status = 400,
                message = "Invalid request"
            }, cancellationToken);

            return true;            
        }

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

        await httpContext.Response.WriteAsJsonAsync(new
        {
            status = 500,
            message = "An unexpected error occurred"
        }, cancellationToken);

        return true;
    }
}