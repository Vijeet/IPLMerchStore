namespace IplMerchStore.Application.DTOs;

/// <summary>
/// Data transfer object for search result items
/// Contains product information relevant to search context
/// </summary>
public class ProductSearchResultDto
{
    /// <summary>
    /// Product ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Product name
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Product description (truncated for search results)
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Product price
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Currency code (e.g., INR)
    /// </summary>
    public string? Currency { get; set; }

    /// <summary>
    /// Current inventory count
    /// </summary>
    public int InventoryCount { get; set; }

    /// <summary>
    /// Product type enum value (1-8)
    /// </summary>
    public int ProductType { get; set; }

    /// <summary>
    /// Product type display name (e.g., "Jersey", "Cap")
    /// </summary>
    public string? ProductTypeLabel { get; set; }

    /// <summary>
    /// Associated franchise ID
    /// </summary>
    public int FranchiseId { get; set; }

    /// <summary>
    /// Associated franchise name
    /// </summary>
    public string? FranchiseName { get; set; }

    /// <summary>
    /// Product image URL
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// SKU (Stock Keeping Unit)
    /// </summary>
    public string? SKU { get; set; }

    /// <summary>
    /// Whether the product is active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Relevance score for the search (0-100)
    /// Higher score indicates better match for the search query
    /// Calculated based on name match vs description match
    /// </summary>
    public int RelevanceScore { get; set; }

    /// <summary>
    /// UTC timestamp when product was created
    /// </summary>
    public DateTime CreatedAtUtc { get; set; }
}
