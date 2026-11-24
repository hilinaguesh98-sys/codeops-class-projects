using System.Diagnostics;

namespace AddisEats.Api.Models;

public sealed class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/problem+json";

            var traceId = Activity.Current?.Id ?? context.TraceIdentifier;
            var problem = ApiProblem.Create(
                500,
                "INTERNAL_SERVER_ERROR",
                "Internal server error",
                "An unexpected server error occurred.",
                context.Request.Path);
            problem.Extensions["traceId"] = traceId;

            await context.Response.WriteAsJsonAsync(
                problem,
                options: new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
                },
                contentType: "application/problem+json");
        }
    }
}
