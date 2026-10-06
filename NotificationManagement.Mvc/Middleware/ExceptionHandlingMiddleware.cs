
using NotificationManagement.Core.Exceptions;

namespace NotificationManagement.Mvc.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
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
        catch (NotFoundException ex)
        {
            _logger.LogWarning(
                ex,
                "Resource not found: {Path}",
                context.Request.Path);

            await HandleExceptionAsync(
                context,
                StatusCodes.Status404NotFound);
        }
        catch (UnsupportedNotificationTypeException ex)
        {
            _logger.LogWarning(
                ex,
                "Unsupported notification type: {Path}",
                context.Request.Path);

            await HandleExceptionAsync(
                context,
                StatusCodes.Status400BadRequest);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unhandled exception: {Method} {Path}",
                context.Request.Method,
                context.Request.Path);

            await HandleExceptionAsync(
                context,
                StatusCodes.Status500InternalServerError);
        }
    }

    private async Task HandleExceptionAsync(
        HttpContext context,
        int statusCode)
    {
        if (context.Response.HasStarted)
            return;

        context.Response.Clear();

        // Preserve the actual HTTP status code.
        context.Response.StatusCode = statusCode;

        // Re-execute the MVC error endpoint internally.
        context.Request.Path = "/Home/Error";
        context.Request.QueryString =
            new QueryString($"?statusCode={statusCode}");

        await _next(context);
    }
}