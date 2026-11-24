using Microsoft.AspNetCore.Mvc;

namespace AddisEats.Api.Controllers;

[ApiController]
[Route("api/v1/failures")]
public sealed class DebugController : ControllerBase
{
    [HttpPost("exception")]
    public IActionResult ThrowException()
    {
        throw new InvalidOperationException(
            "Deliberate exception for global handler verification.");
    }
}
