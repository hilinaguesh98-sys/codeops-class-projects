using AddisEats.Api.Models;
using Microsoft.AspNetCore.Mvc;

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
    public ActionResult<OrderDto> GetById(int id)
    {
        var order = _orders.FirstOrDefault(item => item.Id == id);
        return order is null ? NotFound() : Ok(order);
    }

    [HttpPost]
    public ActionResult<OrderDto> Create([FromBody] CreateOrderRequest request)
    {
        var id = _orders.Count == 0 ? 1 : _orders.Max(item => item.Id) + 1;
        var order = new OrderDto(id, request.RestaurantId, request.TotalEtb, OrderStatus.Pending);
        _orders.Add(order);

        return CreatedAtAction(nameof(GetById), new { id }, order);
    }

    [HttpPatch("{id:int}/status")]
    public ActionResult<OrderDto> UpdateStatus(int id, [FromBody] UpdateOrderStatusRequest request)
    {
        var index = _orders.FindIndex(item => item.Id == id);
        if (index < 0)
        {
            return NotFound();
        }

        var updated = _orders[index] with { Status = request.Status };
        _orders[index] = updated;
        return Ok(updated);
    }
}