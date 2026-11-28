using AddisEats.Api.Options;
using AddisEats.Api.Services;
using AddisEats.Api.Dtos;
using AddisEats.OrderBook.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using AddisEats.Api.Models;

namespace AddisEats.Api.Controllers;

/// <summary>Searches and maintains AddisEats menu items.</summary>
[ApiController]
[Route("api/v1/menu-items")]
[Tags("Menu items")]
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

    /// <summary>Searches menu items by category and optional page.</summary>
    /// <param name="category">Optional menu category, such as Stew or Breakfast.</param>
    /// <param name="page">Optional one-based page number.</param>
    /// <response code="200">Returns the matching menu items.</response>
    /// <response code="400">A filter or query parameter is invalid.</response>
    /// <response code="404">A requested menu item was not found.</response>
    /// <response code="500">An unexpected service error occurred.</response>
    [HttpGet]
    [RejectUnknownQuery("category", "page")]
    [ProducesResponseType(typeof(IReadOnlyList<MenuItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
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
            ? Ok(items.Value!.Select(item => item.ToDto()).ToArray())
            : ProblemResult(items.Error!, HttpContext.Request.Path);
    }

    /// <summary>Gets a menu item by its identifier.</summary>
    /// <param name="id">Unique menu item identifier.</param>
    /// <response code="200">Returns the requested menu item.</response>
    /// <response code="400">The query contains an unsupported parameter.</response>
    /// <response code="404">No menu item exists with this identifier.</response>
    /// <response code="500">An unexpected server error occurred.</response>
    [HttpGet("{id:int}")]
    [RejectUnknownQuery]
    [ProducesResponseType(typeof(MenuItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public IActionResult GetById(int id)
    {
        var menuItem = _menuService.GetMenuItemById(id);
        if (!menuItem.IsSuccess)
        {
            return NotFound(ApiProblem.Create(
                404, menuItem.Error!.Code, "Menu item not found",
                menuItem.Error.Message, HttpContext.Request.Path));
        }

        return Ok(menuItem.Value!.ToDto());
    }

    /// <summary>Adds a menu item to the menu.</summary>
    /// <param name="request">The menu item name, category, price, and identifier.</param>
    /// <response code="201">Returns the created menu item and its resource location.</response>
    /// <response code="400">The request body or query parameters are invalid.</response>
    /// <response code="409">A menu item with this identifier already exists.</response>
    /// <response code="415">The request content type is unsupported.</response>
    /// <response code="500">An unexpected server error occurred.</response>
    [HttpPost]
    [RejectUnknownQuery]
    [ProducesResponseType(typeof(MenuItemDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
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

        var createdItem = result.Value!;
        return CreatedAtAction(nameof(GetById),
            new { id = createdItem.Id }, createdItem.ToDto());
    }
}
