using AddisEats.OrderBook.Models;

namespace AddisEats.Api.Services;

public sealed class MenuStore
{
    private readonly List<MenuItem> _items = new()
    {
        new MenuItem(1, "Shiro", "Stew", 120.50m),
        new MenuItem(2, "Doro Wot", "Stew", 250.00m),
        new MenuItem(3, "Chechebsa", "Breakfast", 95.00m),
        new MenuItem(4, "Tibs", "Main", 320.00m),
        new MenuItem(5, "Kitfo", "Main", 275.00m),
        new MenuItem(6, "Firfir", "Breakfast", 135.00m)
    };

    public IReadOnlyList<MenuItem> GetSnapshot()
    {
        lock (_items)
        {
            return _items.ToArray();
        }
    }

    public bool TryAdd(MenuItem menuItem)
    {
        lock (_items)
        {
            if (_items.Any(item => item.Id == menuItem.Id))
            {
                return false;
            }

            _items.Add(menuItem);
            return true;
        }
    }
}
