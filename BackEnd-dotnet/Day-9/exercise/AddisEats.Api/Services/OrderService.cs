using AddisEats.Api.Models;

namespace AddisEats.Api.Services;

public sealed class OrderService : IOrderService
{
    private static readonly IReadOnlyDictionary<int, decimal> MenuPrices =
        new Dictionary<int, decimal>
        {
            [1] = 120.50m,
            [2] = 250m,
            [3] = 95m,
            [4] = 320m,
            [5] = 275m,
            [6] = 135m
        };

    private readonly List<Order> _orders =
    [
        new(1, 1, 70m, OrderStatus.Pending)
    ];

    public Order? GetById(int id) =>
        _orders.FirstOrDefault(order => order.Id == id);

    public Result<Order> Place(CreateOrderRequest request)
    {
        if (request.RestaurantId is < 1 or > 5)
        {
            return Result<Order>.Failure(
                "RESTAURANT_NOT_FOUND",
                $"Restaurant {request.RestaurantId} was not found.");
        }

        var unknownItem = request.Items
            .Select(item => item.MenuItemId)
            .FirstOrDefault(id => !MenuPrices.ContainsKey(id));
        if (unknownItem != 0)
        {
            return Result<Order>.Failure(
                "MENU_ITEM_NOT_FOUND",
                $"Menu item {unknownItem} was not found.");
        }

        var total = request.Items.Sum(item =>
            MenuPrices[item.MenuItemId] * item.Quantity);
        if (total < 100m)
        {
            return Result<Order>.Failure(
                "MINIMUM_ORDER_TOTAL",
                "The server-computed order total must be at least 100 ETB.");
        }

        var order = new Order(
            _orders.Count == 0 ? 1 : _orders.Max(x => x.Id) + 1,
            request.RestaurantId,
            total,
            OrderStatus.Pending);
        _orders.Add(order);
        return Result<Order>.Success(order);
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
