using System.ComponentModel.DataAnnotations;

namespace AddisEats.Api.Models;

/// <summary>Request body for placing a restaurant order.</summary>
public sealed class CreateOrderRequest
{
    /// <summary>Identifier of the restaurant receiving the order.</summary>
    /// <example>1</example>
    [Range(1, 5)]
    public int RestaurantId { get; init; }

    /// <summary>Menu items and quantities in the order.</summary>
    /// <example>[{"menuItemId":1,"quantity":2}]</example>
    [Required]
    [MinLength(1)]
    public List<OrderLineRequest> Items { get; init; } = [];
}

/// <summary>A menu item and quantity in an order request.</summary>
public sealed class OrderLineRequest
{
    /// <summary>Identifier of the menu item.</summary>
    /// <example>1</example>
    [Range(1, int.MaxValue)]
    public int MenuItemId { get; init; }

    /// <summary>Number of units to order.</summary>
    /// <example>2</example>
    [Range(1, 20)]
    public int Quantity { get; init; }
}
