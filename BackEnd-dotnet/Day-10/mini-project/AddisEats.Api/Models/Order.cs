namespace AddisEats.Api.Models;

using System.ComponentModel.DataAnnotations;

public enum OrderStatus
{
    Pending,
    Accepted,
    Completed,
    Cancelled
}

/// <summary>Details required to place a restaurant order.</summary>
public sealed class CreateOrderRequest : IValidatableObject
{
    /// <summary>Identifier of the restaurant preparing the order.</summary>
    /// <example>1</example>
    [Required(ErrorMessage = "Restaurant ID is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Restaurant ID must be a positive integer.")]
    public int RestaurantId { get; init; }

    /// <summary>Name of the customer receiving the order.</summary>
    /// <example>Abebe Example</example>
    [Required(ErrorMessage = "Customer name is required.")]
    [StringLength(80, MinimumLength = 2,
        ErrorMessage = "Customer name must contain between 2 and 80 characters.")]
    public string CustomerName { get; init; } = string.Empty;

    /// <summary>Ethiopian mobile number for delivery coordination.</summary>
    /// <example>+251911000000</example>
    [Required(ErrorMessage = "Customer phone is required.")]
    [EthiopianMobile(ErrorMessage =
        "Phone must be 09 or 07 with 8 additional digits, or +251 followed by 9 or 7.")]
    [StringLength(13, MinimumLength = 10,
        ErrorMessage = "Customer phone must use a valid Ethiopian mobile format.")]
    public string CustomerPhone { get; init; } = string.Empty;

    /// <summary>Delivery address in Addis Ababa.</summary>
    /// <example>Bole, Addis Ababa, near Edna Mall</example>
    [Required(ErrorMessage = "Delivery address is required.")]
    [MinLength(5, ErrorMessage = "Delivery address must contain at least 5 characters.")]
    [StringLength(200, ErrorMessage = "Delivery address cannot exceed 200 characters.")]
    public string DeliveryAddress { get; init; } = string.Empty;

    /// <summary>Menu items and quantities requested.</summary>
    /// <example>[{"menuItemId":2,"quantity":1}]</example>
    [Required(ErrorMessage = "At least one order line is required.")]
    [MinLength(1, ErrorMessage = "At least one order line is required.")]
    public List<OrderLineRequest> Items { get; init; } = [];

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var units = Items?.Sum(item => item.Quantity) ?? 0;
        if (units > 20)
        {
            yield return new ValidationResult(
                "An order cannot contain more than 20 units.",
                [nameof(Items)]);
        }

        if (RestaurantId == 1 && units > 10)
        {
            yield return new ValidationResult(
                "Restaurant 1 is limited to 10 units per order.",
                [nameof(RestaurantId), nameof(Items)]);
        }
    }
}

/// <summary>A menu item and quantity within an order.</summary>
public sealed class OrderLineRequest
{
    /// <summary>Identifier of the menu item.</summary>
    /// <example>2</example>
    [Range(1, int.MaxValue, ErrorMessage = "Menu item ID must be a positive integer.")]
    public int MenuItemId { get; init; }

    /// <summary>Number of portions requested.</summary>
    /// <example>1</example>
    [Range(1, 20, ErrorMessage = "Quantity must be between 1 and 20.")]
    public int Quantity { get; init; } = 1;
}

/// <summary>Requested status transition for an order.</summary>
public sealed class UpdateOrderStatusRequest : IValidatableObject
{
    /// <summary>New order status.</summary>
    /// <example>Accepted</example>
    [Required(ErrorMessage = "Order status is required.")]
    [EnumDataType(typeof(OrderStatus), ErrorMessage =
        "Status must be Pending, Accepted, Completed, or Cancelled.")]
    public OrderStatus? Status { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Status == OrderStatus.Pending)
        {
            yield return new ValidationResult(
                "Pending is not a valid status transition for this endpoint.",
                [nameof(Status)]);
        }
    }
}

/// <summary>Order details returned by the AddisEats API.</summary>
public sealed record OrderDto
{
    /// <summary>Unique order identifier.</summary>
    /// <example>104</example>
    public int Id { get; init; }

    /// <summary>Restaurant preparing the order.</summary>
    /// <example>1</example>
    public int RestaurantId { get; init; }

    /// <summary>Server-calculated order total in Ethiopian birr.</summary>
    /// <example>500.00</example>
    public decimal TotalEtb { get; init; }

    /// <summary>Current order status.</summary>
    /// <example>Pending</example>
    public OrderStatus Status { get; init; }
}