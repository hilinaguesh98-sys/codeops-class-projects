using AddisEats.Api.Dtos;
using AddisEats.Api.Services;
using Microsoft.AspNetCore.Mvc;
using AddisEats.Api.Models;
using Microsoft.AspNetCore.Http;

namespace AddisEats.Api.Controllers;

[ApiController]
[Route("api/v1/restaurants")]
public sealed class RestaurantsController : ControllerBase
{
    private readonly IRestaurantService _restaurantService;

    public RestaurantsController(IRestaurantService restaurantService)
    {
        _restaurantService = restaurantService;
    }

    [HttpGet]
    [RejectUnknownQuery("area", "areas", "maxFeeEtb", "page", "limit")]
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

        return Ok(_restaurantService.Search(query));
    }

    [HttpGet("{id:int}")]
    [RejectUnknownQuery]
    public IActionResult GetById(int id)
    {
        var restaurant = _restaurantService.GetById(id);
        if (restaurant is null)
        {
            return NotFound(ApiProblem.Create(
                404, "RESTAURANT_NOT_FOUND", "Restaurant not found",
                $"Restaurant {id} was not found.", HttpContext.Request.Path));
        }

        return Ok(restaurant.ToDto());
    }

    [HttpGet("{id:int}/menu-items")]
    [RejectUnknownQuery]
    public IActionResult GetMenu(int id)
    {
        var restaurant = _restaurantService.GetById(id);
        if (restaurant is null)
        {
            return NotFound(ApiProblem.Create(
                404, "RESTAURANT_NOT_FOUND", "Restaurant not found",
                $"Restaurant {id} was not found.", HttpContext.Request.Path));
        }

        return Ok(restaurant.MenuItems);
    }
}
