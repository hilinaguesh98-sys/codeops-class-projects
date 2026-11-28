using AddisEats.Api.Dtos;
using AddisEats.Api.Models;
using AddisEats.OrderBook.Models;

namespace AddisEats.Api.Services;

public sealed class RestaurantService : IRestaurantService
{
    private static readonly IReadOnlyList<Restaurant> Restaurants = CreateRestaurants();

    public IReadOnlyList<RestaurantDto> Search(string? area, int page, int limit)
    {
        if (page < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(page), "Page numbers start at 1.");
        }

        if (limit < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(limit), "Limit must be greater than zero.");
        }

        var restaurants = Restaurants.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(area))
        {
            restaurants = restaurants.Where(restaurant =>
                string.Equals(restaurant.Area, area, StringComparison.OrdinalIgnoreCase));
        }

        var skip = (int)Math.Min((long)(page - 1) * limit, int.MaxValue);

        return restaurants
            .Skip(skip)
            .Take(limit)
            .Select(restaurant => restaurant.ToDto())
            .ToArray();
    }

    public Restaurant? GetById(int id)
    {
        return Restaurants.FirstOrDefault(restaurant => restaurant.Id == id);
    }

    private static IReadOnlyList<Restaurant> CreateRestaurants()
    {
        var shiro = new MenuItem(1, "Shiro", "Stew", 120.50m);
        var doroWot = new MenuItem(2, "Doro Wot", "Stew", 250.00m);
        var chechebsa = new MenuItem(3, "Chechebsa", "Breakfast", 95.00m);
        var tibs = new MenuItem(4, "Tibs", "Main", 320.00m);

        return new[]
        {
            new Restaurant(1, "Bole Injera House", "Bole", "Bole Road", "+251-900-000-001", 35.00m, 0.10m, new[] { shiro, doroWot }),
            new Restaurant(2, "Lomi Kitchen", "Bole", "Atlas Road", "+251-900-000-002", 28.00m, 0.12m, new[] { chechebsa }),
            new Restaurant(3, "Blue Nile Grill", "Bole", "Cameroon Street", "+251-900-000-003", 40.00m, 0.15m, new[] { tibs }),
            new Restaurant(4, "Addis Garden Cafe", "Bole", "Airport Road", "+251-900-000-004", 25.00m, 0.10m, new[] { shiro, chechebsa }),
            new Restaurant(5, "Selam Vegan Table", "Bole", "Bole Medhanialem", "+251-900-000-005", 30.00m, 0.11m, new[] { shiro }),
            new Restaurant(6, "Kazanchis Tibs Corner", "Kazanchis", "Tewodros Square", "+251-900-000-006", 32.00m, 0.12m, new[] { tibs }),
            new Restaurant(7, "Lucy Coffee and Kitchen", "Kazanchis", "Churchill Avenue", "+251-900-000-007", 20.00m, 0.10m, new[] { chechebsa, shiro }),
            new Restaurant(8, "Meskel Family Restaurant", "Kazanchis", "Meskel Square", "+251-900-000-008", 27.00m, 0.13m, new[] { doroWot }),
            new Restaurant(9, "Arada Traditional Kitchen", "Piassa", "Ras Mekonnen Avenue", "+251-900-000-009", 24.00m, 0.11m, new[] { shiro, doroWot }),
            new Restaurant(10, "Piassa Wot House", "Piassa", "Tewodros Street", "+251-900-000-010", 22.00m, 0.10m, new[] { doroWot }),
            new Restaurant(11, "Megenagna Pasta and Grill", "Megenagna", "Megenagna Square", "+251-900-000-011", 35.00m, 0.14m, new[] { tibs }),
            new Restaurant(12, "Megenagna Tea Corner", "Megenagna", "C.M.C. Road", "+251-900-000-012", 18.00m, 0.10m, Array.Empty<MenuItem>())
        };
    }
}
