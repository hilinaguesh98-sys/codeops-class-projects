using AddisEats.Api.Dtos;
using AddisEats.Api.Models;
using AddisEats.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace AddisEats.Api.Controllers;

[ApiController]
[Route("api/v1/restaurants")]
public sealed class RestaurantsController : ControllerBase
{
    private readonly IRestaurantService _service;

    public RestaurantsController(IRestaurantService service)
    {
        _service = service;
    }

    [HttpGet]
    [RejectUnknownQuery("area", "areas", "maxFeeEtb", "page", "limit")]
    public IReadOnlyList<RestaurantDto> Search([FromQuery] RestaurantQuery query)
    {
        return _service.Search(query);
    }

    [HttpGet("{id:int}")]
    [RejectUnknownQuery]
    public ActionResult<RestaurantDto> GetOne(int id)
    {
        var restaurant = _service.GetById(id);
        if (restaurant is null)
        {
            return NotFound();
        }

        return RestaurantService.ToDto(restaurant);
    }

    [HttpGet("{id:int}/menu")]
    [RejectUnknownQuery]
    public IActionResult GetMenu(int id) =>
        _service.GetById(id) is { } restaurant
            ? Ok(new[] { restaurant.Name })
            : NotFound();
}
