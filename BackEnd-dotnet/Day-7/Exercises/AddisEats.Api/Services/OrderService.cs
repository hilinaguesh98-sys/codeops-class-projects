using AddisEats.Api.Models;

namespace AddisEats.Api.Services;

public sealed class OrderService : IOrderService
{
    private readonly List<Order> _orders =
    [
        new(1, 1, 70m, OrderStatus.Pending)
    ];

    public Order? GetById(int id) =>
        _orders.FirstOrDefault(order => order.Id == id);

    public Order Create(CreateOrderRequest request)
    {
        if (request.TotalEtb < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(request.TotalEtb), "The ETB total cannot be negative.");
        }

        var order = new Order(
            _orders.Count == 0 ? 1 : _orders.Max(x => x.Id) + 1,
            request.RestaurantId,
            request.TotalEtb,
            OrderStatus.Pending);
        _orders.Add(order);
        return order;
    }

    public Order UpdateStatus(int id, OrderStatus status)
    {
        var order = GetById(id)
            ?? throw new KeyNotFoundException($"Order {id} was not found.");

        if (order.Status == OrderStatus.Delivered && status == OrderStatus.Cancelled)
        {
            throw new InvalidOperationException("A delivered order cannot be cancelled.");
        }

        var updated = order with { Status = status };
        var index = _orders.FindIndex(x => x.Id == id);
        _orders[index] = updated;
        return updated;
    }
}
