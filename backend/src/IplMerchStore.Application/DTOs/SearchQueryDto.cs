namespace IplMerchStore.Application.DTOs;

/// <summary>
/// Data transfer object for product search queries
/// Supports flexible search with multiple filter options
/// </summary>
public class SearchQueryDto
{
    /// <summary>
    /// Search query text (matches product name and description)
    /// </summary>
    public string? Query { get; set; }

    /// <summary>
    /// Filter by franchise ID
    /// </summary>
    public int? FranchiseId { get; set; }

    /// <summary>
    /// Filter by product type (1-8)
    /// </summary>
    public int? ProductType { get; set; }

    /// <summary>
    /// Page number for pagination (default: 1)
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Page size for pagination (default: 10, max: 100)
    /// </summary>
    public int PageSize { get; set; } = 10;
}
