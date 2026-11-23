using AddisEats.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace AddisEats.Api.Controllers;

[ApiController]
[Route("api/v1/payments/provider")]
public class PaymentProviderController : ControllerBase
{
    private readonly IPaymentProvider _paymentProvider;

    public PaymentProviderController(IPaymentProvider paymentProvider)
    {
        _paymentProvider = paymentProvider;
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new { provider = _paymentProvider.Name });
    }
}
