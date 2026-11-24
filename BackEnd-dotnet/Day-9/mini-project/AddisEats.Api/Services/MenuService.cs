using AddisEats.OrderBook.Models;
using AddisEats.Api.Models;

namespace AddisEats.Api.Services;

public sealed class MenuService : IMenuService
{
    private readonly MenuStore _store;

    public MenuService(MenuStore store)
    {
        _store = store;
    }

    public Result<IReadOnlyList<MenuItem>> GetMenuItems(string? category = null)
    {
        var items = _store.GetSnapshot();

        if (string.IsNullOrWhiteSpace(category))
        {
            return Result<IReadOnlyList<MenuItem>>.Success(items);
        }

        var filtered = items
            .Where(item => string.Equals(item.Category, category, StringComparison.OrdinalIgnoreCase))
            .ToArray();
        return Result<IReadOnlyList<MenuItem>>.Success(filtered);
    }

    public Result<IReadOnlyList<MenuItem>> GetMenuPage(string? category, int pageNumber, int pageSize)
    {
        if (pageNumber < 1)
        {
            return Result<IReadOnlyList<MenuItem>>.Failure(
                "INVALID_PAGE", "Page numbers start at 1.");
        }

        if (pageSize < 1)
        {
            return Result<IReadOnlyList<MenuItem>>.Failure(
                "INVALID_LIMIT", "Page size must be between 1 and 100.");
        }

        if (pageSize > 100)
        {
            return Result<IReadOnlyList<MenuItem>>.Failure(
                "INVALID_LIMIT", "Page size must be between 1 and 100.");
        }

        var offset = (long)(pageNumber - 1) * pageSize;
        var skip = (int)Math.Min(offset, int.MaxValue);

        var items = GetMenuItems(category);
        if (!items.IsSuccess)
        {
            return items;
        }

        var page = items.Value!
            .Skip(skip)
            .Take(pageSize)
            .ToArray();
        return Result<IReadOnlyList<MenuItem>>.Success(page);
    }

    public Result<MenuItem> GetMenuItemById(int id)
    {
        var item = _store.GetSnapshot().FirstOrDefault(x => x.Id == id);
        return item is null
            ? Result<MenuItem>.Failure(
                "MENU_ITEM_NOT_FOUND", $"Menu item {id} was not found.")
            : Result<MenuItem>.Success(item);
    }

    public Result<MenuItem> AddMenuItem(MenuItem menuItem)
    {
        return _store.TryAdd(menuItem)
            ? Result<MenuItem>.Success(menuItem)
            : Result<MenuItem>.Failure(
                "MENU_ITEM_CONFLICT", $"A menu item with ID {menuItem.Id} already exists.");
    }
}
