namespace IplMerchStore.Application.DTOs;

/// <summary>
/// Response DTO for a cart item with product snapshot information
/// </summary>
public class CartItemResponse
{
    /// <summary>
    /// Cart item ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Product ID
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Product name snapshot at time of adding to cart
    /// </summary>
    public string? ProductName { get; set; }

    /// <summary>
    /// Product image URL snapshot at time of adding to cart
    /// </summary>
    public string? ProductImageUrl { get; set; }

    /// <summary>
    /// Product SKU snapshot
    /// </summary>
    public string? ProductSku { get; set; }

    /// <summary>
    /// Unit price of the product at time of adding to cart
    /// Used for historical pricing
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Current quantity of this item in the cart
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Subtotal for this item (UnitPrice * Quantity)
    /// </summary>
    public decimal Subtotal { get; set; }

    /// <summary>
    /// Current available inventory count for the product
    /// Useful for frontend validation on quantity updates
    /// </summary>
    public int? CurrentInventory { get; set; }

    /// <summary>
    /// Whether the product is still active (not discontinued)
    /// </summary>
    public bool IsProductActive { get; set; }
}
