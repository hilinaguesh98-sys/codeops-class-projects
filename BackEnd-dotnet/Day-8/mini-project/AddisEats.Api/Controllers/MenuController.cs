using AddisEats.Api.Options;
using AddisEats.Api.Services;
using AddisEats.OrderBook.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using AddisEats.Api.Models;

namespace AddisEats.Api.Controllers;

[ApiController]
[Route("api/v1/menu-items")]
public sealed class MenuController : ControllerBase
{
    private readonly IMenuService _menuService;
    private readonly MenuOptions _menuOptions;

    public MenuController(IMenuService menuService, IOptions<MenuOptions> menuOptions)
    {
        _menuService = menuService;
        _menuOptions = menuOptions.Value;
    }

    private ObjectResult ProblemResult(Error error, PathString path)
    {
        var status = error.Code switch
        {
            "INVALID_PAGE" or "INVALID_LIMIT" => StatusCodes.Status400BadRequest,
            "MENU_ITEM_NOT_FOUND" => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status500InternalServerError
        };
        return StatusCode(status, ApiProblem.Create(
            status, error.Code, "Request failed", error.Message, path));
    }

    [HttpGet]
    [RejectUnknownQuery("category", "page")]
    public IActionResult Get([FromQuery] string? category, [FromQuery] int? page)
    {
        if (page is < 1)
        {
            return BadRequest(ApiProblem.Create(
                400, "INVALID_PAGE", "Invalid page",
                "Page must be greater than zero.", HttpContext.Request.Path));
        }

        var items = page.HasValue
            ? _menuService.GetMenuPage(category, page.Value, _menuOptions.DefaultPageSize)
            : _menuService.GetMenuItems(category);
        return items.IsSuccess
            ? Ok(items.Value)
            : ProblemResult(items.Error!, HttpContext.Request.Path);
    }

    [HttpGet("{id:int}")]
    [RejectUnknownQuery]
    public IActionResult GetById(int id)
    {
        var menuItem = _menuService.GetMenuItemById(id);
        if (!menuItem.IsSuccess)
        {
            return NotFound(ApiProblem.Create(
                404, menuItem.Error!.Code, "Menu item not found",
                menuItem.Error.Message, HttpContext.Request.Path));
        }

        return Ok(menuItem.Value);
    }

    [HttpPost]
    [RejectUnknownQuery]
    public IActionResult Add([FromBody] CreateMenuItemRequest request)
    {
        var menuItem = new MenuItem(
            request.Id,
            request.Name,
            request.Category,
            request.Price!.Value);

        var result = _menuService.AddMenuItem(menuItem);
        if (!result.IsSuccess)
        {
            return Conflict(ApiProblem.Create(
                409, result.Error!.Code, "Menu item already exists",
                result.Error.Message, HttpContext.Request.Path));
        }

        return CreatedAtAction(nameof(GetById),
            new { id = result.Value!.Id }, result.Value);
    }
}
