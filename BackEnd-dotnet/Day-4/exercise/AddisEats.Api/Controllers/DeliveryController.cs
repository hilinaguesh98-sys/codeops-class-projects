using AddisEats.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AddisEats.Api.Controllers;

[ApiController]
[Route("api/v1/delivery")]
public class DeliveryController : ControllerBase
{
    private readonly DeliveryOptions _options;

    public DeliveryController(IOptions<DeliveryOptions> options)
    {
        _options = options.Value;
    }

    [HttpGet("options")]
    public IActionResult GetOptions()
    {
        return Ok(new
        {
            freeAboveEtb = _options.FreeAboveEtb,
            defaultFeeEtb = _options.DefaultFeeEtb
        });
    }
}
