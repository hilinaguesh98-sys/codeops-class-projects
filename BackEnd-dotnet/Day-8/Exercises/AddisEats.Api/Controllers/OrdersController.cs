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
        return order is null
            ? NotFound(ApiProblem.Create(
                404, "ORDER_NOT_FOUND", "Order not found",
                $"Order {id} was not found.", HttpContext.Request.Path))
            : Ok(order);
    }

    [HttpPost]
    [RejectUnknownQuery]
    public ActionResult<Order> Create(CreateOrderRequest request)
    {
        var result = _service.Place(request);
        if (!result.IsSuccess)
        {
            var error = result.Error!;
            var status = error.Code switch
            {
                "RESTAURANT_NOT_FOUND" => StatusCodes.Status404NotFound,
                "MENU_ITEM_NOT_FOUND" => StatusCodes.Status404NotFound,
                "MINIMUM_ORDER_TOTAL" => StatusCodes.Status422UnprocessableEntity,
                _ => StatusCodes.Status400BadRequest
            };

            var problem = ApiProblem.Create(
                status,
                error.Code,
                "Order could not be placed",
                error.Message,
                HttpContext.Request.Path);
            return status switch
            {
                StatusCodes.Status404NotFound => NotFound(problem),
                StatusCodes.Status422UnprocessableEntity =>
                    UnprocessableEntity(problem),
                _ => BadRequest(problem)
            };
        }

        var order = result.Value!;
        return CreatedAtAction(nameof(Get), new { id = order.Id }, order);
    }

    [HttpPatch("{id:int}")]
    [RejectUnknownQuery]
    public ActionResult<Order> UpdateStatus(int id, [FromBody] UpdateOrderStatusRequest request)
    {
        var existing = _service.GetById(id);
        if (existing is null)
        {
            return NotFound(ApiProblem.Create(
                404, "ORDER_NOT_FOUND", "Order not found",
                $"Order {id} was not found.", HttpContext.Request.Path));
        }

        if (existing.Status == OrderStatus.Delivered && request.Status == OrderStatus.Cancelled)
        {
            return Conflict(ApiProblem.Create(
                409, "ORDER_ALREADY_DELIVERED", "Order cannot be cancelled",
                "A delivered order cannot be cancelled.", HttpContext.Request.Path));
        }

        return Ok(_service.UpdateStatus(id, request.Status));
    }
}
