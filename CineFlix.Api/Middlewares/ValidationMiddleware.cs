using FluentValidation;

namespace CineFlix.Api.Middlewares;

public class ValidationMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException ex)
        {
            var errors = ex.Errors.Select(e => 
                new ValidationError
                {
                    PropertyName = e.PropertyName, ErrorMessage = e.ErrorMessage = e.ErrorMessage
                }).ToList();
            
            
            context.Response.StatusCode = 400;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(errors);
        }
    }
}

public class ValidationError
{
    public string PropertyName { get; init; }
    public string ErrorMessage { get; init; }
}