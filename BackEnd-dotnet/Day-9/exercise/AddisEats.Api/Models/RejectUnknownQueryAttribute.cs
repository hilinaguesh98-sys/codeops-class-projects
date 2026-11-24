using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AddisEats.Api.Models;

[AttributeUsage(AttributeTargets.Method)]
public sealed class RejectUnknownQueryAttribute(params string[] allowedNames) : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var allowed = allowedNames.ToHashSet(StringComparer.OrdinalIgnoreCase);
        var unknown = context.HttpContext.Request.Query.Keys
            .Where(name => !allowed.Contains(name))
            .ToArray();

        if (unknown.Length == 0)
        {
            return;
        }

        context.Result = new BadRequestObjectResult(ApiProblem.Create(
            StatusCodes.Status400BadRequest,
            "UNKNOWN_QUERY_PARAMETER",
            "Unknown query parameter",
            $"Unknown query parameter(s): {string.Join(", ", unknown)}.",
            context.HttpContext.Request.Path));
    }
}
