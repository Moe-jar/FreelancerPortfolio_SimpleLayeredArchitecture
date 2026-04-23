using System.Net;
using System.Text.Json;
using FluentValidation;

namespace FreelancerPortfolio.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            // Sanitize user-provided values to prevent log forging
            var method = SanitizeForLog(context.Request.Method);
            var path = SanitizeForLog(context.Request.Path.ToString());
            _logger.LogError(ex, "Unhandled exception for {Method} {Path}", method, path);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, message, errors) = exception switch
        {
            ValidationException ve => (
                HttpStatusCode.BadRequest,
                "Validation failed.",
                ve.Errors.Select(e => e.ErrorMessage).ToArray()
            ),
            KeyNotFoundException => (HttpStatusCode.NotFound, exception.Message, Array.Empty<string>()),
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized, exception.Message, Array.Empty<string>()),
            InvalidOperationException => (HttpStatusCode.Conflict, exception.Message, Array.Empty<string>()),
            _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred.", Array.Empty<string>())
        };

        context.Response.StatusCode = (int)statusCode;

        var response = new
        {
            success = false,
            message,
            errors,
            statusCode = (int)statusCode,
        };

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }

    private static string SanitizeForLog(string value)
    {
        if (string.IsNullOrEmpty(value)) return "(empty)";
        // Remove newlines and carriage returns to prevent log injection
        return value.Replace("\r", "\\r").Replace("\n", "\\n");
    }
}
