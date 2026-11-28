using System.Collections.Concurrent;
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

    private readonly ILogger<OrderService> _logger;
    private readonly ConcurrentDictionary<int, Order> _orders = new();
    private int _nextOrderId;

    public OrderService(ILogger<OrderService> logger)
    {
        _logger = logger;
    }

    public Order? GetById(int id) =>
        _orders.TryGetValue(id, out var order) ? order : null;

    public Result<Order> Place(CreateOrderRequest request, string correlationId)
    {
        _logger.LogDebug(
            "Placing order for restaurant {RestaurantId} with correlation ID {CorrelationId}",
            request.RestaurantId,
            correlationId);

        if (request.Items.Any(item => !MenuPrices.ContainsKey(item.MenuItemId)))
        {
            return Result<Order>.Failure(
                "MENU_ITEM_NOT_FOUND",
                "One or more menu items were not found.");
        }

        var total = request.Items.Sum(item =>
            MenuPrices[item.MenuItemId] * item.Quantity);
        if (total < 100m)
        {
            return Result<Order>.Failure(
                "MINIMUM_ORDER_TOTAL",
                "The server-computed order total must be at least 100 ETB.");
        }

        var orderId = Interlocked.Increment(ref _nextOrderId);
        var order = new Order(
            orderId,
            request.RestaurantId,
            total,
            OrderStatus.Pending);
        _orders[order.Id] = order;

        _logger.LogInformation(
            "Order {OrderId} created with correlation ID {CorrelationId}",
            order.Id,
            correlationId);

        return Result<Order>.Success(order);
    }
}
