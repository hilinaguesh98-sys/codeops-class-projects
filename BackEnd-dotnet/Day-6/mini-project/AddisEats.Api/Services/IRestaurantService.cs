using AddisEats.Api.Dtos;
using AddisEats.Api.Models;

namespace AddisEats.Api.Services;

public interface IRestaurantService
{
    IReadOnlyList<RestaurantDto> Search(RestaurantQuery query);

    Restaurant? GetById(int id);
}
