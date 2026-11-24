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
        var order = new Order(
            _orders.Count == 0 ? 1 : _orders.Max(x => x.Id) + 1,
            request.RestaurantId,
            request.TotalEtb,
            OrderStatus.Pending);
        _orders.Add(order);
        return order;
    }
}
