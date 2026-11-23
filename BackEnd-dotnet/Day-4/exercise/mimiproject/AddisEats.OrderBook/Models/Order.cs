using System.Linq;

namespace AddisEats.OrderBook.Models;

public sealed class Order
{
    public string Customer { get; }

    public string Area { get; }

    public IReadOnlyList<OrderLine> Lines { get; }

    public decimal Total => Lines.Sum(line => line.MenuItem.Price * line.Quantity);

    public Order(string customer, string area, IEnumerable<OrderLine> lines)
    {
        Customer = customer;
        Area = area;
        Lines = lines?.ToArray() ?? throw new ArgumentException("An order must include its lines.", nameof(lines));
    }
}
