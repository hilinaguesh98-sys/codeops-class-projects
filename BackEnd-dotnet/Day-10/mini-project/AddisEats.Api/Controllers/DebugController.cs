using Microsoft.AspNetCore.Mvc;

namespace AddisEats.Api.Controllers;

/// <summary>Provides diagnostic endpoints for API error handling.</summary>
[ApiController]
[Route("api/v1/failures")]
[Tags("Diagnostics")]
public sealed class DebugController : ControllerBase
{
    /// <summary>Triggers the global exception handler for verification.</summary>
    /// <response code="500">Returns a safe problem response with the request correlation ID.</response>
    [HttpPost("exception")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public IActionResult ThrowException()
    {
        throw new InvalidOperationException(
            "Deliberate exception for global handler verification.");
    }
}
