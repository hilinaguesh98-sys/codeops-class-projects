using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace AddisEats.Api.Models;

public sealed class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(
        RequestDelegate next,
        ILogger<ExceptionMiddleware> logger)
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
        catch (Exception exception)
        {
            var traceId = Activity.Current?.Id ?? context.TraceIdentifier;
            _logger.LogError(
                exception,
                "Unhandled request exception. TraceId={TraceId}",
                traceId);

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            var problem = ApiProblem.Create(
                StatusCodes.Status500InternalServerError,
                "INTERNAL_SERVER_ERROR",
                "Internal server error",
                "An unexpected server error occurred.",
                context.Request.Path);
            problem.Extensions["traceId"] = traceId;

            context.Response.ContentType = "application/problem+json";
            await context.Response.WriteAsJsonAsync(problem);
        }
    }
}
