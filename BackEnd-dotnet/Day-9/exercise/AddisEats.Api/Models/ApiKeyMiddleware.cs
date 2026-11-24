using System.Text.Json;

namespace AddisEats.Api.Models;

public sealed class ApiKeyMiddleware
{
    public const string HeaderName = "X-Admin-Key";
    private readonly RequestDelegate _next;

    public ApiKeyMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue(HeaderName, out var supplied) ||
            supplied != "admin-secret")
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/problem+json";
            var problem = ApiProblem.Create(
                StatusCodes.Status401Unauthorized,
                "UNAUTHENTICATED",
                "Admin API key is required",
                "Provide a valid X-Admin-Key header.",
                context.Request.Path);
            await context.Response.WriteAsJsonAsync(problem);
            return;
        }

        await _next(context);
    }
}
