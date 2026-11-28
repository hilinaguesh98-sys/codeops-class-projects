using System.Diagnostics;

namespace AddisEats.Api.Models;

public sealed class RequestTimingMiddleware
{
    public const long WarningThresholdMilliseconds = 1000;
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

        var elapsed = stopwatch.ElapsedMilliseconds;

        if (elapsed > WarningThresholdMilliseconds)
        {
            _logger.LogWarning(
                "Request completed. CorrelationId={CorrelationId} Method={Method} Status={StatusCode} ElapsedMs={ElapsedMilliseconds}",
                context.TraceIdentifier,
                context.Request.Method,
                context.Response.StatusCode,
                elapsed);
        }
        else
        {
            _logger.LogInformation(
                "Request completed. CorrelationId={CorrelationId} Method={Method} Status={StatusCode} ElapsedMs={ElapsedMilliseconds}",
                context.TraceIdentifier,
                context.Request.Method,
                context.Response.StatusCode,
                elapsed);
        }
    }
}
