using Microsoft.AspNetCore.Mvc;
using AddisEats.Api.Services;

namespace AddisEats.Api.Controllers;

[ApiController]
[Route("api/v1/health")]
public class HealthController : ControllerBase
{
    private readonly IHostEnvironment _environment;
    private readonly IClock _clock;
    private readonly IClock _secondClock;

    public HealthController(IHostEnvironment environment, IClock clock, IClock secondClock)
    {
        _environment = environment;
        _clock = clock;
        _secondClock = secondClock;
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            utcTime = _clock.UtcNow,
            secondUtcTime = _secondClock.UtcNow,
            environment = _environment.EnvironmentName
        });
    }
}
