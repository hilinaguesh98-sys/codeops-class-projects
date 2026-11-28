namespace AddisEats.Api.Models;

using System.ComponentModel.DataAnnotations;

/// <summary>Query filters for finding Addis Ababa restaurants.</summary>
public sealed class RestaurantQuery
{
    /// <summary>Filter by one neighborhood.</summary>
    /// <example>Bole</example>
    [StringLength(50, ErrorMessage = "Area cannot exceed 50 characters.")]
    public string? Area { get; set; }

    /// <summary>Filter by multiple neighborhoods.</summary>
    /// <example>["Bole","Piassa"]</example>
    public List<string> Areas { get; set; } = [];

    /// <summary>Maximum delivery fee in Ethiopian birr.</summary>
    /// <example>35.00</example>
    [EtbAmount(ErrorMessage =
        "Maximum delivery fee must be positive and have no more than two decimal places.")]
    public decimal? MaxFeeEtb { get; set; }

    /// <summary>One-based result page.</summary>
    /// <example>1</example>
    [Range(1, int.MaxValue, ErrorMessage = "Page must be greater than zero.")]
    public int Page { get; set; } = 1;

    /// <summary>Number of restaurants per page, from 1 to 100.</summary>
    /// <example>5</example>
    [Range(1, 100, ErrorMessage = "Limit must be between 1 and 100.")]
    public int Limit { get; set; } = 10;
}