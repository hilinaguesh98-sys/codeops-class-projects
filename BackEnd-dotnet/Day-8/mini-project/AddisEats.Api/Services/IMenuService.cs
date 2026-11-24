using AddisEats.OrderBook.Models;
using AddisEats.Api.Models;

namespace AddisEats.Api.Services;

public interface IMenuService
{
    Result<IReadOnlyList<MenuItem>> GetMenuItems(string? category = null);

    Result<IReadOnlyList<MenuItem>> GetMenuPage(string? category, int pageNumber, int pageSize);

    Result<MenuItem> GetMenuItemById(int id);

    Result<MenuItem> AddMenuItem(MenuItem menuItem);
}
