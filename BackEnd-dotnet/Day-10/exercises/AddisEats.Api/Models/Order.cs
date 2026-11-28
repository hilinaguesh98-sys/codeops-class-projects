namespace AddisEats.Api.Models;

/// <summary>Current state of an order.</summary>
public enum OrderStatus
{
    Pending,
    Accepted,
    Completed,
    Delivered,
    Cancelled
}

/// <summary>Order details returned by the API.</summary>
public sealed record Order(
    int Id,
    int RestaurantId,
    decimal TotalEtb,
    OrderStatus Status);
