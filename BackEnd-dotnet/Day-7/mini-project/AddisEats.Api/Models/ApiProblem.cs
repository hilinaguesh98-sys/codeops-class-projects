using Microsoft.AspNetCore.Mvc;

namespace AddisEats.Api.Models;

public static class ApiProblem
{
    public static ProblemDetails Create(
        int status,
        string code,
        string title,
        string detail,
        string? instance = null)
    {
        var problem = new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = detail,
            Instance = instance,
            Type = $"https://httpstatuses.com/{status}"
        };
        problem.Extensions["code"] = code;
        return problem;
    }
}
