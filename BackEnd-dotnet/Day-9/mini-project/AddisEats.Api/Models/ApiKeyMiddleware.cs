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
        if (!context.Request.Headers.TryGetValue(HeaderName, out var key) ||
            key != "admin-secret")
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/problem+json";
            await context.Response.WriteAsJsonAsync(ApiProblem.Create(
                StatusCodes.Status401Unauthorized,
                "UNAUTHENTICATED",
                "Admin API key is required",
                "Provide a valid X-Admin-Key header.",
                context.Request.Path));
            return;
        }

        await _next(context);
    }
}
