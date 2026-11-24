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

        return Ok(items);
    }

    [HttpGet("{id:int}")]
    [RejectUnknownQuery]
    public IActionResult GetById(int id)
    {
        var menuItem = _menuService.GetMenuItemById(id);
        if (menuItem is null)
        {
            return NotFound(ApiProblem.Create(
                404, "MENU_ITEM_NOT_FOUND", "Menu item not found",
                $"Menu item {id} was not found.", HttpContext.Request.Path));
        }

        return Ok(menuItem);
    }

    [HttpPost]
    [RejectUnknownQuery]
    public IActionResult Add([FromBody] MenuItem menuItem)
    {
        if (menuItem.Price <= 0m)
        {
            return BadRequest(ApiProblem.Create(
                400, "INVALID_PRICE", "Invalid menu item price",
                "Menu item price must be greater than zero.", HttpContext.Request.Path));
        }

        if (!_menuService.AddMenuItem(menuItem))
        {
            return Conflict(ApiProblem.Create(
                409, "MENU_ITEM_CONFLICT", "Menu item already exists",
                $"A menu item with ID {menuItem.Id} already exists.", HttpContext.Request.Path));
        }

        return CreatedAtAction(nameof(GetById), new { id = menuItem.Id }, menuItem);
    }
}
