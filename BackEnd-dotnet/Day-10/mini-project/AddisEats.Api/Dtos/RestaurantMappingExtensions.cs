using AddisEats.Api.Models;

namespace AddisEats.Api.Dtos;

public static class RestaurantMappingExtensions
{
    public static RestaurantDto ToDto(this Restaurant restaurant)
    {
        return new RestaurantDto
        {
            Id = restaurant.Id,
            Name = restaurant.Name,
            Area = restaurant.Area,
            Address = restaurant.Address,
            DeliveryFeeEtb = restaurant.DeliveryFeeEtb
        };
    }
}
