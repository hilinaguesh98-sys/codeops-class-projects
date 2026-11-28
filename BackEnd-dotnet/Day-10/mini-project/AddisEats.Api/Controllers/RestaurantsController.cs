using AddisEats.Api.Dtos;
using AddisEats.Api.Services;
using Microsoft.AspNetCore.Mvc;
using AddisEats.Api.Models;
using Microsoft.AspNetCore.Http;

namespace AddisEats.Api.Controllers;

/// <summary>Searches restaurants and their menus in Addis Ababa.</summary>
[ApiController]
[Route("api/v1/restaurants")]
[Tags("Restaurants")]
public sealed class RestaurantsController : ControllerBase
{
    private readonly IRestaurantService _restaurantService;

    public RestaurantsController(IRestaurantService restaurantService)
    {
        _restaurantService = restaurantService;
    }

    private ObjectResult ProblemResult(Error error, PathString path)
    {
        var status = error.Code switch
        {
            "INVALID_PAGE" or "INVALID_LIMIT" => StatusCodes.Status400BadRequest,
            "RESTAURANT_NOT_FOUND" => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status500InternalServerError
        };
        return StatusCode(status, ApiProblem.Create(
            status, error.Code, "Request failed", error.Message, path));
    }

    /// <summary>Searches restaurants by neighborhood and delivery fee.</summary>
    /// <param name="query">Pagination and restaurant search filters.</param>
    /// <response code="200">Returns the matching restaurants.</response>
    /// <response code="400">A filter or query parameter is invalid.</response>
    /// <response code="404">A requested restaurant was not found.</response>
    /// <response code="500">An unexpected service error occurred.</response>
    [HttpGet]
    [RejectUnknownQuery("area", "areas", "maxFeeEtb", "page", "limit")]
    [ProducesResponseType(typeof(IReadOnlyList<RestaurantDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public ActionResult<IReadOnlyList<RestaurantDto>> Search([FromQuery] RestaurantQuery query)
    {
        if (query.Page < 1)
        {
            return BadRequest(ApiProblem.Create(
                400, "INVALID_PAGE", "Invalid page", "Page must be greater than zero.",
                HttpContext.Request.Path));
        }

        if (query.Limit < 1 || query.Limit > 100)
        {
            return BadRequest(ApiProblem.Create(
                400, "INVALID_LIMIT", "Invalid limit",
                "Limit must be between 1 and 100.",
                HttpContext.Request.Path));
        }

        var result = _restaurantService.Search(query);
        return result.IsSuccess
            ? Ok(result.Value)
            : ProblemResult(result.Error!, HttpContext.Request.Path);
    }

    /// <summary>Gets restaurant details by identifier.</summary>
    /// <param name="id">Unique restaurant identifier.</param>
    /// <response code="200">Returns the restaurant details.</response>
    /// <response code="400">The query contains an unsupported parameter.</response>
    /// <response code="404">No restaurant exists with this identifier.</response>
    /// <response code="500">An unexpected server error occurred.</response>
    [HttpGet("{id:int}")]
    [RejectUnknownQuery]
    [ProducesResponseType(typeof(RestaurantDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public IActionResult GetById(int id)
    {
        var result = _restaurantService.GetById(id);
        return result.IsSuccess
            ? Ok(result.Value!.ToDto())
            : NotFound(ApiProblem.Create(
                404, result.Error!.Code, "Restaurant not found",
                result.Error.Message, HttpContext.Request.Path));
    }

    /// <summary>Gets all menu items for a restaurant.</summary>
    /// <param name="id">Unique restaurant identifier.</param>
    /// <response code="200">Returns the restaurant menu.</response>
    /// <response code="400">The query contains an unsupported parameter.</response>
    /// <response code="404">No restaurant exists with this identifier.</response>
    /// <response code="500">An unexpected server error occurred.</response>
    [HttpGet("{id:int}/menu-items")]
    [RejectUnknownQuery]
    [ProducesResponseType(typeof(IReadOnlyList<MenuItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public IActionResult GetMenu(int id)
    {
        var result = _restaurantService.GetById(id);
        if (!result.IsSuccess)
        {
            return NotFound(ApiProblem.Create(
                404, result.Error!.Code, "Restaurant not found",
                result.Error.Message, HttpContext.Request.Path));
        }

        return Ok(result.Value!.MenuItems.Select(item => item.ToDto()).ToArray());
    }
}
