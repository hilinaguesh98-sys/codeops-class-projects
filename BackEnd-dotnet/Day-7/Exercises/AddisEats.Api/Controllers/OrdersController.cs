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
    [RejectUnknownQuery]
    public ActionResult<Order> Get(int id)
    {
        var order = _service.GetById(id);
        return order is null ? NotFound() : Ok(order);
    }

    [HttpPost]
    [RejectUnknownQuery]
    public ActionResult<Order> Create(CreateOrderRequest request)
    {
        if (request.TotalEtb < 0)
        {
            return BadRequest(new
            {
                code = "INVALID_TOTAL",
                message = "The ETB total must not be negative."
            });
        }

        var order = _service.Create(request);
        return CreatedAtAction(nameof(Get), new { id = order.Id }, order);
    }

    [HttpPatch("{id:int}")]
    [RejectUnknownQuery]
    public ActionResult<Order> UpdateStatus(int id, [FromBody] UpdateOrderStatusRequest request)
    {
        var existing = _service.GetById(id);
        if (existing is null)
        {
            return NotFound(new
            {
                code = "ORDER_NOT_FOUND",
                message = $"Order {id} was not found."
            });
        }

        if (existing.Status == OrderStatus.Delivered && request.Status == OrderStatus.Cancelled)
        {
            return Conflict(new
            {
                code = "ORDER_ALREADY_DELIVERED",
                message = "A delivered order cannot be cancelled."
            });
        }

        return Ok(_service.UpdateStatus(id, request.Status));
    }
}
