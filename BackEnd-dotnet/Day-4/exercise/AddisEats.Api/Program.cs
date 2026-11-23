using AddisEats.Api.Authentication;
using AddisEats.Api.Services;
using AddisEats.Api.Models;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSingleton<AddisEats.Api.Services.IClock, AddisEats.Api.Services.SystemClock>();
builder.Services.AddScoped<IPaymentProvider, CbeBirrProvider>();
builder.Services.Configure<DeliveryOptions>(builder.Configuration.GetSection("Delivery"));
builder.Services.AddAuthentication("Exercise")
    .AddScheme<AuthenticationSchemeOptions, ExerciseAuthenticationHandler>("Exercise", _ => { });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
