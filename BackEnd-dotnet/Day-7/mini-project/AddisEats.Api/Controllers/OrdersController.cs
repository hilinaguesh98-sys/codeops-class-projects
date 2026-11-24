using AddisEats.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace AddisEats.Api.Controllers;

[ApiController]
[Route("api/v1/orders")]
public sealed class OrdersController : ControllerBase
{
    private readonly List<OrderDto> _orders =
    [
        new(1, 1, 70m, OrderStatus.Pending)
    ];

    [HttpGet("{id:int}")]
    [RejectUnknownQuery]
    public ActionResult<OrderDto> GetById(int id)
    {
        var order = _orders.FirstOrDefault(item => item.Id == id);
        return order is null
            ? NotFound(ApiProblem.Create(
                404, "ORDER_NOT_FOUND", "Order not found",
                $"Order {id} was not found.", HttpContext.Request.Path))
            : Ok(order);
    }

    [HttpPost]
    [RejectUnknownQuery]
    public ActionResult<OrderDto> Create([FromBody] CreateOrderRequest request)
    {
        if (request.TotalEtb < 0)
        {
            return BadRequest(ApiProblem.Create(
                400, "INVALID_TOTAL", "Invalid order total",
                "Order total must not be negative.", HttpContext.Request.Path));
        }

        var id = _orders.Count == 0 ? 1 : _orders.Max(item => item.Id) + 1;
        var order = new OrderDto(id, request.RestaurantId, request.TotalEtb, OrderStatus.Pending);
        _orders.Add(order);

        return CreatedAtAction(nameof(GetById), new { id }, order);
    }

    [HttpPatch("{id:int}")]
    [RejectUnknownQuery]
    public IActionResult UpdateStatus(int id, [FromBody] UpdateOrderStatusRequest request)
    {
        var index = _orders.FindIndex(item => item.Id == id);
        if (index < 0)
        {
            return NotFound(ApiProblem.Create(
                404, "ORDER_NOT_FOUND", "Order not found",
                $"Order {id} was not found.", HttpContext.Request.Path));
        }

        var existing = _orders[index];
        if (existing.Status == OrderStatus.Completed &&
            request.Status == OrderStatus.Cancelled)
        {
            return Conflict(ApiProblem.Create(
                409, "ORDER_ALREADY_COMPLETED", "Order cannot be cancelled",
                "An already completed order cannot be cancelled.",
                HttpContext.Request.Path));
        }

        var updated = _orders[index] with { Status = request.Status };
        _orders[index] = updated;
        return NoContent();
    }
}