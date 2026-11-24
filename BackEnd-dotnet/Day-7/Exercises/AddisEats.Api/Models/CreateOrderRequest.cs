namespace AddisEats.Api.Models;

public sealed record CreateOrderRequest(int RestaurantId, decimal TotalEtb);
