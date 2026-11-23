using AddisEats.OrderBook.Models;

namespace AddisEats.Api.Services;

public interface IMenuService
{
    IReadOnlyList<MenuItem> GetMenuItems(string? category = null);

    IReadOnlyList<MenuItem> GetMenuPage(string? category, int pageNumber, int pageSize);

    MenuItem? GetMenuItemById(int id);

    bool AddMenuItem(MenuItem menuItem);
}
