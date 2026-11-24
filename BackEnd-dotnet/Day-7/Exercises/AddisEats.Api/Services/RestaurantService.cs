using AddisEats.Api.Dtos;
using AddisEats.Api.Models;

namespace AddisEats.Api.Services;

public sealed class RestaurantService : IRestaurantService
{
    private static readonly IReadOnlyList<Restaurant> Restaurants =
    [
        new(1, "Bole Injera House", "Bole", "Bole Road", 35m, 0.10m),
        new(2, "Lomi Kitchen", "Bole", "Atlas Road", 28m),
        new(3, "Blue Nile Grill", "Piassa", "Cameroon Street", 40m, 0.15m),
        new(4, "Arada Traditional Kitchen", "Piassa", "Ras Mekonnen Avenue", 24m),
        new(5, "Megenagna Tea Corner", "Megenagna", "C.M.C. Road", 18m)
    ];

    public IReadOnlyList<RestaurantDto> Search(RestaurantQuery query)
    {
        var restaurants = Restaurants.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(query.Area))
        {
            restaurants = restaurants.Where(x =>
                string.Equals(x.Area, query.Area, StringComparison.OrdinalIgnoreCase));
        }

        if (query.Areas.Count > 0)
        {
            restaurants = restaurants.Where(x =>
                query.Areas.Any(area =>
                    string.Equals(x.Area, area, StringComparison.OrdinalIgnoreCase)));
        }

        if (query.MaxFeeEtb.HasValue)
        {
            restaurants = restaurants.Where(x => x.DeliveryFeeEtb <= query.MaxFeeEtb.Value);
        }

        var skip = (query.Page - 1) * query.Limit;
        return restaurants
            .Skip(Math.Max(skip, 0))
            .Take(query.Limit)
            .Select(ToDto)
            .ToArray();
    }

    public Restaurant? GetById(int id) =>
        Restaurants.FirstOrDefault(x => x.Id == id);

    public static RestaurantDto ToDto(Restaurant restaurant) =>
        new(
            restaurant.Id,
            restaurant.Name,
            restaurant.Area,
            restaurant.Address,
            restaurant.DeliveryFeeEtb,
            restaurant.CommissionRate);
}
