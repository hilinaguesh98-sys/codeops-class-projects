namespace AddisEats.Api.Options;

/// <summary>Configuration for menu browsing.</summary>
public sealed class MenuOptions
{
    /// <summary>Number of menu items shown on a page when no page size is provided.</summary>
    /// <example>5</example>
    public int DefaultPageSize { get; set; } = 5;
}
