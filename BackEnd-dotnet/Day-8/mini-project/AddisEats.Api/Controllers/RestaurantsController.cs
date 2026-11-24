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

        var result = _restaurantService.Search(query);
        return result.IsSuccess
            ? Ok(result.Value)
            : ProblemResult(result.Error!, HttpContext.Request.Path);
    }

    [HttpGet("{id:int}")]
    [RejectUnknownQuery]
    public IActionResult GetById(int id)
    {
        var result = _restaurantService.GetById(id);
        return result.IsSuccess
            ? Ok(result.Value!.ToDto())
            : NotFound(ApiProblem.Create(
                404, result.Error!.Code, "Restaurant not found",
                result.Error.Message, HttpContext.Request.Path));
    }

    [HttpGet("{id:int}/menu-items")]
    [RejectUnknownQuery]
    public IActionResult GetMenu(int id)
    {
        var result = _restaurantService.GetById(id);
        if (!result.IsSuccess)
        {
            return NotFound(ApiProblem.Create(
                404, result.Error!.Code, "Restaurant not found",
                result.Error.Message, HttpContext.Request.Path));
        }

        return Ok(result.Value!.MenuItems);
    }
}
