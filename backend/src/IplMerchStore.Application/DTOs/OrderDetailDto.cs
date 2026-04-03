namespace IplMerchStore.Application.DTOs;

/// <summary>
/// Response DTO for detailed order information with all items
/// </summary>
public class OrderDetailDto
{
    /// <summary>
    /// Order ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// User ID who placed the order
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// Total amount of the order
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Order status (enum value)
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// Shipping address for the order
    /// </summary>
    public string? ShippingAddress { get; set; }

    /// <summary>
    /// Customer email
    /// </summary>
    public string? CustomerEmail { get; set; }

    /// <summary>
    /// Customer phone number
    /// </summary>
    public string? CustomerPhone { get; set; }

    /// <summary>
    /// Order creation timestamp (UTC)
    /// </summary>
    public DateTime CreatedAtUtc { get; set; }

    /// <summary>
    /// Collection of items in the order
    /// </summary>
    public IEnumerable<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();

    /// <summary>
    /// Subtotal amount (before any taxes/fees)
    /// </summary>
    public decimal SubTotal { get; set; }

    /// <summary>
    /// Order item count
    /// </summary>
    public int ItemCount { get; set; }
}
