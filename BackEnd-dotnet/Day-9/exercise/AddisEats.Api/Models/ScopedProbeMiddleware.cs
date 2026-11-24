namespace AddisEats.Api.Models;

public sealed class ScopedProbeMiddleware
{
    private readonly RequestDelegate _next;

    public ScopedProbeMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context,
        ScopedProbeService probe)
    {
        context.Response.Headers["X-Scoped-Probe"] =
            probe.ScopeId.ToString("N");
        await _next(context);
    }
}
