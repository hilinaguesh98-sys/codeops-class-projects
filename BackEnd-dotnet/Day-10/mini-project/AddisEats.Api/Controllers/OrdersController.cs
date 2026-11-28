using AddisEats.Api.Models;
using AddisEats.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace AddisEats.Api.Controllers;

/// <summary>Creates, retrieves, and updates AddisEats orders.</summary>
[ApiController]
[Route("api/v1/orders")]
[Tags("Orders")]
public sealed class OrdersController : ControllerBase
{
    private readonly IOrderService _service;

    public OrdersController(IOrderService service)
    {
        _service = service;
    }

    /// <summary>Gets an order by its identifier.</summary>
    /// <param name="id">Unique order identifier.</param>
    /// <response code="200">Returns the requested order.</response>
    /// <response code="400">The query contains an unsupported parameter.</response>
    /// <response code="404">No order exists with this identifier.</response>
    /// <response code="500">An unexpected server error occurred.</response>
    [HttpGet("{id:int}")]
    [RejectUnknownQuery]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public ActionResult<OrderDto> GetById(int id)
    {
        var order = _service.GetById(id);
        return !order.IsSuccess
            ? NotFound(ApiProblem.Create(
                404, order.Error!.Code, "Order not found",
                order.Error.Message, HttpContext.Request.Path))
            : Ok(order.Value);
    }

    /// <summary>Creates an order using server-calculated menu prices.</summary>
    /// <param name="request">Customer, restaurant, and menu item details.</param>
    /// <response code="201">Returns the created order and its resource location.</response>
    /// <response code="400">The request body or query parameters are invalid.</response>
    /// <response code="404">The restaurant or a menu item does not exist.</response>
    /// <response code="422">The calculated order total is below the minimum.</response>
    /// <response code="415">The request content type is unsupported.</response>
    /// <response code="500">An unexpected server error occurred.</response>
    [HttpPost]
    [RejectUnknownQuery]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
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

    /// <summary>Updates the status of an existing order.</summary>
    /// <param name="id">Unique order identifier.</param>
    /// <param name="request">The requested status transition.</param>
    /// <response code="204">The order status was updated.</response>
    /// <response code="400">The request body or query parameters are invalid.</response>
    /// <response code="404">No order exists with this identifier.</response>
    /// <response code="409">The requested transition conflicts with the order state.</response>
    /// <response code="415">The request content type is unsupported.</response>
    /// <response code="500">An unexpected server error occurred.</response>
    [HttpPatch("{id:int}")]
    [RejectUnknownQuery]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
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