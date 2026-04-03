namespace IplMerchStore.Application.DTOs;

/// <summary>
/// Response DTO for a user's cart
/// </summary>
public class CartResponse
{
    /// <summary>
    /// Cart ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// User ID this cart belongs to
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// Collection of items in the cart (empty if no items)
    /// </summary>
    public IEnumerable<CartItemResponse> Items { get; set; } = new List<CartItemResponse>();

    /// <summary>
    /// Count of unique items in cart
    /// </summary>
    public int ItemCount { get; set; }

    /// <summary>
    /// Total quantity across all items
    /// </summary>
    public int TotalQuantity { get; set; }

    /// <summary>
    /// Total cart amount (sum of all item subtotals)
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Currency code (typically "INR")
    /// </summary>
    public string Currency { get; set; } = "INR";

    /// <summary>
    /// Whether the cart is empty
    /// </summary>
    public bool IsEmpty { get; set; }
}
