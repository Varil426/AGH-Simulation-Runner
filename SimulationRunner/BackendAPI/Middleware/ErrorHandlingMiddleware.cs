using Application.Errors;
using System.Net;
using System.Text.Json;

namespace BackendAPI.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _logger = logger;
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleException(context, ex, _logger);
        }
    }

    private async Task HandleException(HttpContext context, Exception exception, ILogger<ErrorHandlingMiddleware> logger)
    {
        object? errors = null;
        switch (exception)
        {
            case RestException restException:
                logger.LogError(restException, "REST ERROR");
                errors = restException.Errors;
                context.Response.StatusCode = (int)restException.HttpStatusCode;
                break;
            default:
                logger.LogError(exception, "SERVER ERROR");
                errors = string.IsNullOrWhiteSpace(exception.Message) ? "ERROR" : exception.Message;
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                break;
        }

        context.Response.ContentType = "application/json";
        if (errors != null)
        {
            var results = JsonSerializer.Serialize(new { errors });
            await context.Response.WriteAsync(results);
        }
    }
}