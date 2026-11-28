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
        _logger.LogInformation(
            "Executing action {ActionName}. CorrelationId={CorrelationId}",
            context.ActionDescriptor.DisplayName,
            context.HttpContext.TraceIdentifier);

        await next();
    }
}
