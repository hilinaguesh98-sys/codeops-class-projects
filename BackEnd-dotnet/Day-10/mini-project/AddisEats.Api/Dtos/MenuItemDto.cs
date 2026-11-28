namespace AddisEats.Api.Dtos;

/// <summary>Menu item details returned by the AddisEats API.</summary>
public sealed record MenuItemDto
{
    /// <summary>Unique menu item identifier.</summary>
    /// <example>2</example>
    public int Id { get; init; }

    /// <summary>Name of the prepared dish.</summary>
    /// <example>Doro Wot</example>
    public required string Name { get; init; }

    /// <summary>Menu category.</summary>
    /// <example>Stew</example>
    public required string Category { get; init; }

    /// <summary>Price in Ethiopian birr.</summary>
    /// <example>250.00</example>
    public decimal Price { get; init; }
}
