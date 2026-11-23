using Microsoft.AspNetCore.Mvc;
using AddisEats.Api.Services;

namespace AddisEats.Api.Controllers;

[ApiController]
[Route("api/v1/health")]
public class HealthController : ControllerBase
{
    private readonly IHostEnvironment _environment;
    private readonly IClock _clock;

    public HealthController(IHostEnvironment environment, IClock clock)
    {
        _environment = environment;
        _clock = clock;
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            utcTime = _clock.UtcNow,
            environment = _environment.EnvironmentName
        });
    }
}
