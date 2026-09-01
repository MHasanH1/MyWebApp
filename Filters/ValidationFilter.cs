using FluentValidation;

namespace MyWebApp.Filters;

public class ValidationFilter<T> : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var validator = context.HttpContext.RequestServices
            .GetService<IValidator<T>>();

        if (validator == null)
        {
            return await next(context);
        }

        var argument = context.Arguments
            .OfType<T>()
            .FirstOrDefault();

        if (argument == null)
        {
            return await next(context);
        }

        var result = await validator.ValidateAsync(argument);

        if (!result.IsValid)
        {
            return Results.BadRequest(new
            {
                errors = result.Errors.Select(error => new
                {
                    field = error.PropertyName,
                    message = error.ErrorMessage
                })
            });
        }

        return await next(context);
    }
}