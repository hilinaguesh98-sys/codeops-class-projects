namespace AddisEats.Api.Models;

public enum OrderStatus
{
    Pending,
    Accepted,
    Completed,
    Cancelled
}

public sealed record CreateOrderRequest(
    int RestaurantId,
    decimal TotalEtb);

public sealed record UpdateOrderStatusRequest(
    OrderStatus Status);

public sealed record OrderDto(
    int Id,
    int RestaurantId,
    decimal TotalEtb,
    OrderStatus Status);