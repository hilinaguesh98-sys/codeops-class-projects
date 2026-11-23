using AddisEats.OrderBook.Models;

namespace AddisEats.OrderBook.Services;

public sealed class OrderBook
{
    private const int PageSize = 5;
    private readonly List<Order> _orders = new();

    public void Add(Order order)
    {
        _orders.Add(order ?? throw new ArgumentNullException(nameof(order)));
    }

    public IReadOnlyList<Order> GetOrdersForArea(string area)
    {
        return _orders
            .Where(order => string.Equals(order.Area, area, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(order => order.Total)
            .ToList();
    }

    public decimal GetTotalRevenue()
    {
        return _orders.Sum(order => order.Total);
    }

    public string? GetBusiestCategory()
    {
        return _orders
            .SelectMany(order => order.Lines)
            .GroupBy(line => line.MenuItem.Category)
            .Select(group => new
            {
                Category = group.Key,
                Quantity = group.Sum(line => line.Quantity)
            })
            .OrderByDescending(group => group.Quantity)
            .ThenBy(group => group.Category, StringComparer.OrdinalIgnoreCase)
            .Select(group => group.Category)
            .FirstOrDefault();
    }

    public IReadOnlyList<Order> GetPage(int pageNumber)
    {
        if (pageNumber < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(pageNumber), "Page numbers start at 1.");
        }

        var offset = (long)(pageNumber - 1) * PageSize;
        var skip = (int)Math.Min(offset, int.MaxValue);

        return _orders
            .Skip(skip)
            .Take(PageSize)
            .ToList();
    }
}
