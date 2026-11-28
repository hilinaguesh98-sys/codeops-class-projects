using System.Diagnostics;

namespace AddisEats.Api.Models;

public sealed class CorrelationIdMiddleware
{
    public const string HeaderName = "X-Correlation-Id";
    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var suppliedId = context.Request.Headers[HeaderName].FirstOrDefault();
        var correlationId = Guid.TryParse(suppliedId, out var parsedId)
            ? parsedId.ToString("D")
            : Guid.NewGuid().ToString("D");

        context.TraceIdentifier = correlationId;
        context.Response.Headers[HeaderName] = correlationId;
        Activity.Current?.SetTag("correlation.id", correlationId);

        await _next(context);
    }
}
