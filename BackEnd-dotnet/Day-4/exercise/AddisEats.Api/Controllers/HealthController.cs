using Microsoft.AspNetCore.Mvc;

namespace AddisEats.Api.Controllers;

[ApiController]
[Route("api/v1/health")]
public class HealthController : ControllerBase
{
    private readonly IHostEnvironment _environment;

    public HealthController(IHostEnvironment environment)
    {
        _environment = environment;
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            utcTime = DateTime.UtcNow,
            environment = _environment.EnvironmentName
        });
    }
}
