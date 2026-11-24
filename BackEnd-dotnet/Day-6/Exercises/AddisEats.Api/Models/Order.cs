namespace AddisEats.Api.Models;

public enum OrderStatus
{
    Pending,
    Accepted,
    Completed
}

public sealed record Order(
    int Id,
    int RestaurantId,
    decimal TotalEtb,
    OrderStatus Status);
