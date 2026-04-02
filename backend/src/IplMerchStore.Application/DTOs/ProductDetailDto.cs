namespace IplMerchStore.Application.DTOs;

/// <summary>
/// Data transfer object for detailed product view with franchise information
/// </summary>
public class ProductDetailDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string? Currency { get; set; }
    public int InventoryCount { get; set; }
    public string? ProductType { get; set; }
    public int FranchiseId { get; set; }
    public string? FranchiseName { get; set; }
    public string? FranchiseShortCode { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; }
    public string? SKU { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}
