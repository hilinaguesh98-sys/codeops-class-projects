namespace AddisEats.Api.Models;

public enum OrderStatus
{
    Pending,
    Accepted,
    Completed,
    Delivered,
    Cancelled
}

public sealed record Order(
    int Id,
    int RestaurantId,
    decimal TotalEtb,
    OrderStatus Status);
