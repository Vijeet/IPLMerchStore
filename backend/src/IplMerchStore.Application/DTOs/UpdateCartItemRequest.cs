namespace IplMerchStore.Application.DTOs;

/// <summary>
/// Request DTO for updating cart item quantity
/// </summary>
public class UpdateCartItemRequest
{
    /// <summary>
    /// New quantity for the cart item
    /// Set to 0 to remove the item from cart
    /// Must be >= 0
    /// </summary>
    public required int Quantity { get; set; }
}
