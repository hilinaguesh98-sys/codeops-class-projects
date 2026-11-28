using System.Reflection;
using AddisEats.Api.Options;
using AddisEats.Api.Services;
using AddisEats.Api.OpenApi;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using AddisEats.Api.Models;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.AddFilter(
    "Microsoft.AspNetCore.HttpsPolicy.HttpsRedirectionMiddleware",
    LogLevel.None);

// Add services to the container.

builder.Services.AddControllers(options =>
    {
        options.Filters.Add<AuditActionFilter>();
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var problem = AddisEats.Api.Models.ApiProblem.Create(
            StatusCodes.Status400BadRequest,
            "INVALID_MODEL",
            "Invalid request",
            "The request body or parameters are invalid.",
            context.HttpContext.Request.Path);
        return new BadRequestObjectResult(problem);
    };
});
builder.Services.AddSingleton<MenuStore>();
builder.Services.AddScoped<AuditActionFilter>();
builder.Services.AddSingleton<IMenuService, MenuService>();
builder.Services.AddScoped<IRestaurantService, RestaurantService>();
builder.Services.AddSingleton<IOrderService, OrderService>();
builder.Services.Configure<MenuOptions>(builder.Configuration.GetSection("Menu"));
var frontendOrigin = builder.Configuration["Cors:FrontendOrigin"]
    ?? throw new InvalidOperationException("Configure Cors:FrontendOrigin for the frontend.");
builder.Services.AddCors(options => options.AddPolicy("Frontend", policy =>
    policy.WithOrigins(frontendOrigin)
        .AllowAnyHeader()
        .AllowAnyMethod()));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "AddisEats API",
        Description = "Restaurant discovery, menus, and order management for Addis Ababa.",
        Version = "v1"
    });

    options.SchemaFilter<OrderStatusSchemaFilter>();
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFile));
});

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<RequestTimingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("Frontend");

// Future: app.UseRateLimiter(); // after CORS and before authentication

// Future: app.UseAuthentication(); // before authorization

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
