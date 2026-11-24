namespace AddisEats.Api.Models;

using System.ComponentModel.DataAnnotations;

public sealed class CreateOrderRequest : IValidatableObject
{
    [Required(ErrorMessage = "Restaurant ID is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Restaurant ID must be a positive integer.")]
    public int RestaurantId { get; init; }

    [Required(ErrorMessage = "Customer name is required.")]
    [StringLength(80, MinimumLength = 2,
        ErrorMessage = "Customer name must contain between 2 and 80 characters.")]
    public string CustomerName { get; init; } = string.Empty;

    [Required(ErrorMessage = "Customer phone is required.")]
    [EthiopianMobile(ErrorMessage =
        "Phone must use 09 or 07 with 8 additional digits, or +251 followed by 9 or 7.")]
    [StringLength(13, MinimumLength = 10,
        ErrorMessage = "Customer phone must use a valid Ethiopian mobile format.")]
    public string CustomerPhone { get; init; } = string.Empty;

    [Required(ErrorMessage = "Delivery address is required.")]
    [MinLength(5, ErrorMessage = "Delivery address must contain at least 5 characters.")]
    [StringLength(200, ErrorMessage = "Delivery address cannot exceed 200 characters.")]
    public string DeliveryAddress { get; init; } = string.Empty;

    [Required(ErrorMessage = "At least one order line is required.")]
    [MinLength(1, ErrorMessage = "At least one order line is required.")]
    public List<OrderLineRequest> Items { get; init; } = [];

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var totalQuantity = Items?.Sum(item => item.Quantity) ?? 0;
        if (totalQuantity > 20)
        {
            yield return new ValidationResult(
                "A restaurant order cannot contain more than 20 units.",
                [nameof(Items)]);
        }

        if (RestaurantId == 1 && totalQuantity > 10)
        {
            yield return new ValidationResult(
                "Restaurant 1 is limited to 10 units per order.",
                [nameof(RestaurantId), nameof(Items)]);
        }
    }
}

public sealed class OrderLineRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "Menu item ID must be a positive integer.")]
    public int MenuItemId { get; init; }

    [Range(1, 20, ErrorMessage = "Quantity must be between 1 and 20.")]
    public int Quantity { get; init; } = 1;

    [EtbAmount(ErrorMessage =
        "ETB amount must be positive and have no more than two decimal places.")]
    public decimal? ValidationAmountEtb { get; init; }
}
