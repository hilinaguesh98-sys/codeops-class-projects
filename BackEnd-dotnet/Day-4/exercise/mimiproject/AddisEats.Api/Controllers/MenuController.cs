using AddisEats.Api.Options;
using AddisEats.Api.Services;
using AddisEats.OrderBook.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AddisEats.Api.Controllers;

[ApiController]
[Route("api/v1/menu")]
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
    public IActionResult Get([FromQuery] string? category, [FromQuery] int? page)
    {
        if (page is < 1)
        {
            return BadRequest(new { message = "Page numbers start at 1." });
        }

        var items = page.HasValue
            ? _menuService.GetMenuPage(category, page.Value, _menuOptions.DefaultPageSize)
            : _menuService.GetMenuItems(category);

        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        var menuItem = _menuService.GetMenuItemById(id);
        if (menuItem is null)
        {
            return NotFound();
        }

        return Ok(menuItem);
    }

    [HttpPost]
    public IActionResult Add([FromBody] MenuItem menuItem)
    {
        if (menuItem.Price <= 0m)
        {
            return BadRequest(new { message = "Menu item price must be greater than zero." });
        }

        if (!_menuService.AddMenuItem(menuItem))
        {
            return Conflict(new { message = $"A menu item with ID {menuItem.Id} already exists." });
        }

        return CreatedAtAction(nameof(GetById), new { id = menuItem.Id }, menuItem);
    }
}
