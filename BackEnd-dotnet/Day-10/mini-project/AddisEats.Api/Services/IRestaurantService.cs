using AddisEats.Api.Dtos;
using AddisEats.Api.Models;

namespace AddisEats.Api.Services;

public interface IRestaurantService
{
    Result<IReadOnlyList<RestaurantDto>> Search(RestaurantQuery query);

    Result<Restaurant> GetById(int id);
}
