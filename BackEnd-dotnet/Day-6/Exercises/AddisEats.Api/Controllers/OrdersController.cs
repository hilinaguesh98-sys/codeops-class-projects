using AddisEats.Api.Models;
using AddisEats.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace AddisEats.Api.Controllers;

[ApiController]
[Route("api/v1/orders")]
public sealed class OrdersController : ControllerBase
{
    private readonly IOrderService _service;

    public OrdersController(IOrderService service)
    {
        _service = service;
    }

    [HttpGet("{id:int}")]
    [ActionName("GetOrder")]
    public ActionResult<Order> Get(int id)
    {
        var order = _service.GetById(id);
        return order is null ? NotFound() : Ok(order);
    }

    [HttpPost]
    public ActionResult<Order> Create(CreateOrderRequest request)
    {
        var order = _service.Create(request);
        return CreatedAtAction(nameof(Get), new { id = order.Id }, order);
    }
}
