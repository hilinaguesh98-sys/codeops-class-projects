using AddisEats.Api.Dtos;
using AddisEats.Api.Services;
using Microsoft.AspNetCore.Mvc;
using AddisEats.Api.Models;

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
    public ActionResult<IReadOnlyList<RestaurantDto>> Search([FromQuery] RestaurantQuery query)
    {
        if (query.Page < 1 || query.Limit < 1)
        {
            return BadRequest(new { message = "Page and limit must be greater than zero." });
        }

        return Ok(_restaurantService.Search(query));
    }

    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        var restaurant = _restaurantService.GetById(id);
        if (restaurant is null)
        {
            return NotFound();
        }

        return Ok(restaurant.ToDto());
    }

    [HttpGet("{id:int}/menu")]
    public IActionResult GetMenu(int id)
    {
        var restaurant = _restaurantService.GetById(id);
        if (restaurant is null)
        {
            return NotFound();
        }

        return Ok(restaurant.MenuItems);
    }
}
