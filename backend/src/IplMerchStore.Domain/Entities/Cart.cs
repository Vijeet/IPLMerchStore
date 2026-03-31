using IplMerchStore.Domain.Common;

namespace IplMerchStore.Domain.Entities;

/// <summary>
/// Represents a shopping cart for a user
/// </summary>
public class Cart : BaseEntity
{
    public required string UserId { get; set; }
    public decimal TotalPrice { get; set; }

    // Navigation properties
    public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
}
