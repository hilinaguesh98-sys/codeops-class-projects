using AddisEats.Api.Dtos;
using AddisEats.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using AddisEats.Api.Options;

namespace AddisEats.Api.Controllers;

[ApiController]
[Route("api/v1/restaurants")]
public sealed class RestaurantsController : ControllerBase
{
    private readonly IRestaurantService _restaurantService;
    private readonly int _defaultLimit;

    public RestaurantsController(
        IRestaurantService restaurantService,
        IOptions<MenuOptions> menuOptions)
    {
        _restaurantService = restaurantService;
        _defaultLimit = menuOptions.Value.DefaultPageSize;
    }

    [HttpGet]
    public IActionResult Search(
        [FromQuery] string? area,
        [FromQuery] int page = 1,
        [FromQuery] int? limit = null)
    {
        var pageLimit = limit ?? _defaultLimit;
        if (page < 1 || pageLimit < 1)
        {
            return BadRequest(new { message = "Page and limit must be greater than zero." });
        }

        return Ok(_restaurantService.Search(area, page, pageLimit));
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
