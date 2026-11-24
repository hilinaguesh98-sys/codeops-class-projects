using System.Diagnostics;

namespace AddisEats.Api.Models;

public sealed class RequestTimingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestTimingMiddleware> _logger;

    public RequestTimingMiddleware(
        RequestDelegate next,
        ILogger<RequestTimingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        await _next(context);
        stopwatch.Stop();

        var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
        var message =
            "Request completed correlationId={CorrelationId} method={Method} path={Path} status={Status} elapsedMs={ElapsedMs}";

        if (elapsedMilliseconds > 1000)
        {
            _logger.LogWarning(
                message,
                context.TraceIdentifier,
                context.Request.Method,
                context.Request.Path.Value,
                context.Response.StatusCode,
                elapsedMilliseconds);
        }
        else
        {
            _logger.LogInformation(
                message,
                context.TraceIdentifier,
                context.Request.Method,
                context.Request.Path.Value,
                context.Response.StatusCode,
                elapsedMilliseconds);
        }
    }
}
