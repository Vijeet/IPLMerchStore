namespace IplMerchStore.Application.DTOs;

/// <summary>
/// Data transfer object for creating or updating a product
/// </summary>
public class ProductInputDto
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required decimal Price { get; set; }
    public string Currency { get; set; } = "INR";
    public required int InventoryCount { get; set; }
    public required int ProductType { get; set; }
    public required int FranchiseId { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; } = true;
    public required string SKU { get; set; }
}
