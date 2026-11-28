using AddisEats.Api.Dtos;
using AddisEats.Api.Models;

namespace AddisEats.Api.Services;

public interface IRestaurantService
{
    IReadOnlyList<RestaurantDto> Search(string? area, int page, int limit);

    Restaurant? GetById(int id);
}
