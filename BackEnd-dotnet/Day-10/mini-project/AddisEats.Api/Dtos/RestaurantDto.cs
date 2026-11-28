namespace AddisEats.Api.Dtos;

/// <summary>Restaurant details returned by the AddisEats API.</summary>
public sealed record RestaurantDto
{
    /// <summary>Unique restaurant identifier.</summary>
    /// <example>1</example>
    public required int Id { get; init; }

    /// <summary>Restaurant display name.</summary>
    /// <example>Bole Injera House</example>
    public required string Name { get; init; }

    /// <summary>Addis Ababa neighborhood where the restaurant is located.</summary>
    /// <example>Bole</example>
    public required string Area { get; init; }

    /// <summary>Restaurant street or landmark address.</summary>
    /// <example>Bole Road, near Edna Mall</example>
    public required string Address { get; init; }

    /// <summary>Delivery fee in Ethiopian birr.</summary>
    /// <example>35.00</example>
    public decimal DeliveryFeeEtb { get; init; }
}
