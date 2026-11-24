namespace AddisEats.Api.Dtos;

public sealed record RestaurantDto(
    int Id,
    string Name,
    string Area,
    string Address,
    decimal DeliveryFeeEtb,
    decimal? CommissionRate);
