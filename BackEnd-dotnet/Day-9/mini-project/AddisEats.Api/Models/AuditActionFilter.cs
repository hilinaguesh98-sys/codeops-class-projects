using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AddisEats.Api.Models;

public sealed class AuditActionFilter : IAsyncActionFilter
{
    private readonly ILogger<AuditActionFilter> _logger;

    public AuditActionFilter(ILogger<AuditActionFilter> logger)
    {
        _logger = logger;
    }

    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        var arguments = JsonSerializer.Serialize(context.ActionArguments);
        _logger.LogInformation(
            "Audit action={ActionName} correlationId={CorrelationId} arguments={Arguments}",
            context.ActionDescriptor.DisplayName,
            context.HttpContext.TraceIdentifier,
            arguments);

        await next();
    }
}
