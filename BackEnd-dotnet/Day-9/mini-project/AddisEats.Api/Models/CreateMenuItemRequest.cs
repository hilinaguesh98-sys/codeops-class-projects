using System.ComponentModel.DataAnnotations;

namespace AddisEats.Api.Models;

public sealed class CreateMenuItemRequest : IValidatableObject
{
    [Range(1, int.MaxValue, ErrorMessage = "Menu item ID must be a positive integer.")]
    public int Id { get; init; }

    [Required(ErrorMessage = "Menu item name is required.")]
    [StringLength(100, MinimumLength = 2,
        ErrorMessage = "Menu item name must contain between 2 and 100 characters.")]
    public string Name { get; init; } = string.Empty;

    [Required(ErrorMessage = "Menu category is required.")]
    [StringLength(50, MinimumLength = 2,
        ErrorMessage = "Menu category must contain between 2 and 50 characters.")]
    public string Category { get; init; } = string.Empty;

    [EtbAmount(ErrorMessage =
        "Menu item price must be positive and have no more than two decimal places.")]
    [Required(ErrorMessage = "Menu item price is required.")]
    public decimal? Price { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.Equals(Category?.Trim(), "Stew", StringComparison.OrdinalIgnoreCase) &&
            Price < 50m)
        {
            yield return new ValidationResult(
                "Stew menu items must cost at least 50 ETB.",
                [nameof(Price), nameof(Category)]);
        }
    }
}
