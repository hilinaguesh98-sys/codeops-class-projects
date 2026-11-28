using AddisEats.Api.Models;

namespace AddisEats.Api.Services;

public sealed class OrderService : IOrderService
{
    private readonly IMenuService _menuService;
    private readonly object _ordersLock = new();
    private readonly List<OrderDto> _orders =
    [
        new()
        {
            Id = 1,
            RestaurantId = 1,
            TotalEtb = 70m,
            Status = OrderStatus.Pending
        }
    ];

    public OrderService(IMenuService menuService)
    {
        _menuService = menuService;
    }

    public Result<OrderDto> GetById(int id)
    {
        lock (_ordersLock)
        {
            var order = _orders.FirstOrDefault(x => x.Id == id);
            return order is null
                ? Result<OrderDto>.Failure(
                    "ORDER_NOT_FOUND", $"Order {id} was not found.")
                : Result<OrderDto>.Success(order);
        }
    }

    public Result<OrderDto> Place(CreateOrderRequest request)
    {
        var restaurant = request.RestaurantId;
        if (restaurant is < 1 or > 12)
        {
            return Result<OrderDto>.Failure(
                "RESTAURANT_NOT_FOUND",
                $"Restaurant {restaurant} was not found.");
        }

        decimal total = 0m;
        foreach (var line in request.Items)
        {
            var menuResult = _menuService.GetMenuItemById(line.MenuItemId);
            if (!menuResult.IsSuccess)
            {
                return Result<OrderDto>.Failure(
                    menuResult.Error!.Code,
                    menuResult.Error.Message);
            }

            total += menuResult.Value!.Price * line.Quantity;
        }

        if (total < 100m)
        {
            return Result<OrderDto>.Failure(
                "MINIMUM_ORDER_TOTAL",
                "The server-computed order total must be at least 100 ETB.");
        }

        OrderDto order;
        lock (_ordersLock)
        {
            var id = _orders.Count == 0 ? 1 : _orders.Max(x => x.Id) + 1;
            order = new OrderDto
            {
                Id = id,
                RestaurantId = restaurant,
                TotalEtb = total,
                Status = OrderStatus.Pending
            };
            _orders.Add(order);
        }

        return Result<OrderDto>.Success(order);
    }

    public Result<OrderDto> UpdateStatus(int id, UpdateOrderStatusRequest request)
    {
        lock (_ordersLock)
        {
            var index = _orders.FindIndex(x => x.Id == id);
            if (index < 0)
            {
                return Result<OrderDto>.Failure(
                    "ORDER_NOT_FOUND", $"Order {id} was not found.");
            }

            var existing = _orders[index];
            if (existing.Status == OrderStatus.Completed &&
                request.Status == OrderStatus.Cancelled)
            {
                return Result<OrderDto>.Failure(
                    "ORDER_ALREADY_COMPLETED",
                    "An already completed order cannot be cancelled.");
            }

            var updated = existing with { Status = request.Status!.Value };
            _orders[index] = updated;
            return Result<OrderDto>.Success(updated);
        }
    }
}
