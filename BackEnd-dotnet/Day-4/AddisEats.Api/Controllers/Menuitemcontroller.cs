using AddisEats.Api.Services.menuitemservices;
using Microsoft.AspNetCore.Mvc;

namespace AddisEats.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MenuItemController : ControllerBase
{
    private readonly IMenuItemServices _menuItemService;

    public MenuItemController(IMenuItemServices menuItemService)
    {
        _menuItemService = menuItemService;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_menuItemService.GetAll());
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var menuitem = _menuItemService.GetById(id);
        if (menuitem == null)
        {
            return NotFound();
        }

        return Ok(menuitem);
    }
}
