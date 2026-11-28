using AddisEats.Api.Models;
using AddisEats.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace AddisEats.Api.Controllers;

/// <summary>Creates and retrieves restaurant orders.</summary>
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
    /// <param name="id">The identifier of the order.</param>
    /// <response code="200">Returns the requested order.</response>
    /// <response code="404">No order exists with this identifier.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Order), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public ActionResult<Order> Get(int id)
    {
        var order = _service.GetById(id);
        return order is null
            ? NotFound(CreateProblem(
                StatusCodes.Status404NotFound,
                "Order not found",
                "No order exists with the requested identifier."))
            : Ok(order);
    }

    /// <summary>Creates an order using server-side menu prices.</summary>
    /// <param name="request">Restaurant and menu item details for the order.</param>
    /// <response code="201">Returns the created order and its resource location.</response>
    /// <response code="400">The request body failed validation.</response>
    /// <response code="404">One or more menu items do not exist.</response>
    /// <response code="422">The order total is below the minimum.</response>
    [HttpPost]
    [ProducesResponseType(typeof(Order), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public ActionResult<Order> Create([FromBody] CreateOrderRequest request)
    {
        var result = _service.Place(request, HttpContext.TraceIdentifier);
        if (!result.IsSuccess)
        {
            var error = result.Error!;
            var statusCode = error.Code switch
            {
                "MENU_ITEM_NOT_FOUND" => StatusCodes.Status404NotFound,
                "MINIMUM_ORDER_TOTAL" => StatusCodes.Status422UnprocessableEntity,
                _ => throw new InvalidOperationException(
                    $"Unsupported order error code: {error.Code}.")
            };
            var problem = CreateProblem(statusCode, "Order could not be placed", error.Message);

            return statusCode switch
            {
                StatusCodes.Status404NotFound => NotFound(problem),
                StatusCodes.Status422UnprocessableEntity => UnprocessableEntity(problem),
                _ => BadRequest(problem)
            };
        }

        var order = result.Value!;
        return CreatedAtAction(nameof(Get), new { id = order.Id }, order);
    }

    private ProblemDetails CreateProblem(int statusCode, string title, string detail) =>
        new()
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = HttpContext.Request.Path
        };
}
