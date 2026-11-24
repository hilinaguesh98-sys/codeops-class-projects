using AddisEats.Api.Models;
using AddisEats.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

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
    public ActionResult<OrderDto> GetById(int id)
    {
        var order = _service.GetById(id);
        return !order.IsSuccess
            ? NotFound(ApiProblem.Create(
                404, order.Error!.Code, "Order not found",
                order.Error.Message, HttpContext.Request.Path))
            : Ok(order.Value);
    }

    [HttpPost]
    [RejectUnknownQuery]
    public ActionResult<OrderDto> Create([FromBody] CreateOrderRequest request)
    {
        var result = _service.Place(request);
        if (!result.IsSuccess)
        {
            var status = result.Error!.Code switch
            {
                "RESTAURANT_NOT_FOUND" or "MENU_ITEM_NOT_FOUND" =>
                    StatusCodes.Status404NotFound,
                "MINIMUM_ORDER_TOTAL" =>
                    StatusCodes.Status422UnprocessableEntity,
                _ => StatusCodes.Status400BadRequest
            };
            return StatusCode(status, ApiProblem.Create(
                status, result.Error.Code, "Order could not be placed",
                result.Error.Message, HttpContext.Request.Path));
        }

        return CreatedAtAction(nameof(GetById),
            new { id = result.Value!.Id }, result.Value);
    }

    [HttpPatch("{id:int}")]
    [RejectUnknownQuery]
    public IActionResult UpdateStatus(int id, [FromBody] UpdateOrderStatusRequest request)
    {
        var result = _service.UpdateStatus(id, request);
        if (!result.IsSuccess)
        {
            var status = result.Error!.Code switch
            {
                "ORDER_NOT_FOUND" => StatusCodes.Status404NotFound,
                "ORDER_ALREADY_COMPLETED" => StatusCodes.Status409Conflict,
                _ => StatusCodes.Status400BadRequest
            };
            return StatusCode(status, ApiProblem.Create(
                status, result.Error.Code, "Order status could not be updated",
                result.Error.Message, HttpContext.Request.Path));
        }
        return NoContent();
    }
}