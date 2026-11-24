using System.Text.Json.Serialization;
using AddisEats.Api.Services;
using AddisEats.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
    {
        options.Filters.Add<AuditActionFilter>();
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.UnmappedMemberHandling =
            System.Text.Json.Serialization.JsonUnmappedMemberHandling.Disallow;
    });
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var problem = ApiProblem.Create(
            StatusCodes.Status400BadRequest,
            "INVALID_MODEL",
            "Invalid request",
            "The request body or parameters are invalid.",
            context.HttpContext.Request.Path);
        problem.Extensions["errors"] = context.ModelState
            .Where(entry => entry.Value?.Errors.Count > 0)
            .ToDictionary(
                entry => entry.Key,
                entry => entry.Value!.Errors.Select(error =>
                    error.ErrorMessage).ToArray());
        return new BadRequestObjectResult(problem);
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthorization();
builder.Services
    .AddAuthentication("Test")
    .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>(
        "Test",
        _ => { });
builder.Services.AddSingleton<IRestaurantService, RestaurantService>();
builder.Services.AddSingleton<IOrderService, OrderService>();
builder.Services.AddScoped<ScopedProbeService>();
builder.Services.AddScoped<AuditActionFilter>();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<RequestTimingMiddleware>();
app.UseMiddleware<ScopedProbeMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.Map("/admin", adminApp =>
{
    adminApp.UseMiddleware<ApiKeyMiddleware>();
    adminApp.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status200OK;
        await context.Response.WriteAsJsonAsync(new
        {
            branch = "admin",
            correlationId = context.TraceIdentifier
        });
    });
});

app.MapControllers();

app.Run();
