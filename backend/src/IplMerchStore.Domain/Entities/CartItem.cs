using IplMerchStore.Domain.Common;

namespace IplMerchStore.Domain.Entities;

/// <summary>
/// Represents an item in a shopping cart
/// </summary>
public class CartItem : BaseEntity
{
    public int CartId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }

    // Navigation properties
    public Cart? Cart { get; set; }
    public Product? Product { get; set; }
}
