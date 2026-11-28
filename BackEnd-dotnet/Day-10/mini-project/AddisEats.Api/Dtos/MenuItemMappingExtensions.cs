using AddisEats.OrderBook.Models;

namespace AddisEats.Api.Dtos;

public static class MenuItemMappingExtensions
{
    public static MenuItemDto ToDto(this MenuItem menuItem) =>
        new()
        {
            Id = menuItem.Id,
            Name = menuItem.Name,
            Category = menuItem.Category,
            Price = menuItem.Price
        };
}
