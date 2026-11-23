using System.ComponentModel.DataAnnotations;

namespace AddisEats.OrderBook.Models;

public sealed record MenuItem
{
    public int Id { get; init; }

    public string Name { get; init; }

    public string Category { get; init; }

    [Range(typeof(decimal), "0.0000000000000000000000000001", "79228162514264337593543950335")]
    public decimal Price { get; init; }

    public MenuItem(int id, string name, string category, decimal price)
    {
        if (price <= 0m)
        {
            throw new ArgumentException("Menu item price must be greater than zero.", nameof(price));
        }

        Id = id;
        Name = name;
        Category = category;
        Price = price;
    }
}
