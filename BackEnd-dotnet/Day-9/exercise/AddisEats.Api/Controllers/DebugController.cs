using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace AddisEats.Api.Controllers;

[ApiController]
[Route("api/v1/failures")]
public sealed class DebugController : ControllerBase
{
    [Authorize]
    [HttpGet("protected")]
    public IActionResult Protected()
    {
        return Ok(new { message = "Authorized" });
    }

    [HttpPost("exception")]
    public IActionResult ThrowException()
    {
        throw new InvalidOperationException("Deliberate exception verification.");
    }
}
