namespace IplMerchStore.Application.DTOs;

/// <summary>
/// Request DTO for adding an item to the cart
/// </summary>
public class AddCartItemRequest
{
    /// <summary>
    /// Product ID to add to cart
    /// </summary>
    public required int ProductId { get; set; }

    /// <summary>
    /// Quantity of the product to add (must be > 0)
    /// If product already exists in cart, this quantity is added to existing quantity
    /// </summary>
    public required int Quantity { get; set; }
}
