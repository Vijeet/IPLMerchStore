using IplMerchStore.Domain.Common;
using IplMerchStore.Domain.Enums;

namespace IplMerchStore.Domain.Entities;

/// <summary>
/// Represents a merchandise product
/// </summary>
public class Product : BaseEntity
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required decimal Price { get; set; }
    public required string Currency { get; set; } = "INR";
    public int InventoryCount { get; set; }
    public ProductType ProductType { get; set; }
    public int FranchiseId { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; } = true;
    public required string SKU { get; set; }

    // Navigation properties
    public Franchise? Franchise { get; set; }
    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
