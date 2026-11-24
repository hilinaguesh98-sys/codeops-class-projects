namespace AddisEats.Api.Models;

using System.ComponentModel.DataAnnotations;

public sealed class RestaurantQuery
{
    [StringLength(50, ErrorMessage = "Area cannot exceed 50 characters.")]
    public string? Area { get; set; }

    public List<string> Areas { get; set; } = [];

    [EtbAmount(ErrorMessage =
        "Maximum delivery fee must be positive and have no more than two decimal places.")]
    public decimal? MaxFeeEtb { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Page must be greater than zero.")]
    public int Page { get; set; } = 1;

    [Range(1, 100, ErrorMessage = "Limit must be between 1 and 100.")]
    public int Limit { get; set; } = 10;
}