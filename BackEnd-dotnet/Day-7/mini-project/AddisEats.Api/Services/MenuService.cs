using AddisEats.OrderBook.Models;

namespace AddisEats.Api.Services;

public sealed class MenuService : IMenuService
{
    private readonly MenuStore _store;

    public MenuService(MenuStore store)
    {
        _store = store;
    }

    public IReadOnlyList<MenuItem> GetMenuItems(string? category = null)
    {
        var items = _store.GetSnapshot();

        if (string.IsNullOrWhiteSpace(category))
        {
            return items;
        }

        return items
            .Where(item => string.Equals(item.Category, category, StringComparison.OrdinalIgnoreCase))
            .ToArray();
    }

    public IReadOnlyList<MenuItem> GetMenuPage(string? category, int pageNumber, int pageSize)
    {
        if (pageNumber < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(pageNumber), "Page numbers start at 1.");
        }

        if (pageSize < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be greater than zero.");
        }

        var offset = (long)(pageNumber - 1) * pageSize;
        var skip = (int)Math.Min(offset, int.MaxValue);

        return GetMenuItems(category)
            .Skip(skip)
            .Take(pageSize)
            .ToArray();
    }

    public MenuItem? GetMenuItemById(int id)
    {
        return _store.GetSnapshot().FirstOrDefault(item => item.Id == id);
    }

    public bool AddMenuItem(MenuItem menuItem)
    {
        return _store.TryAdd(menuItem);
    }
}
