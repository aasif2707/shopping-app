using System.Net;
using System.Text.Json;
using FluentValidation;

namespace IdentityManagement.API.Middleware;

/// <summary>
/// Catches unhandled exceptions, logs them, and returns a consistent ProblemDetails-style JSON response.
/// </summary>
public class GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, title, errors) = exception switch
        {
            ValidationException ve => (
                HttpStatusCode.BadRequest,
                "Validation failed",
                ve.Errors.Select(e => e.ErrorMessage).ToArray()),
            UnauthorizedAccessException => (
                HttpStatusCode.Unauthorized,
                "Unauthorized",
                new[] { "You are not authorized to perform this action." }),
            KeyNotFoundException => (
                HttpStatusCode.NotFound,
                "Resource not found",
                new[] { exception.Message }),
            _ => (
                HttpStatusCode.InternalServerError,
                "An unexpected error occurred",
                new[] { "An internal server error has occurred." })
        };

        if (statusCode == HttpStatusCode.InternalServerError)
            logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);
        else
            logger.LogWarning("Handled exception: {Message}", exception.Message);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = new
        {
            status = (int)statusCode,
            title,
            errors,
            traceId = context.TraceIdentifier
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
