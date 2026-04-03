namespace IplMerchStore.Application.DTOs;

/// <summary>
/// Request DTO for checkout operation
/// </summary>
public class CheckoutRequestDto
{
    /// <summary>
    /// Shipping address for the order
    /// </summary>
    public string? ShippingAddress { get; set; }

    /// <summary>
    /// Customer email address
    /// </summary>
    public string? CustomerEmail { get; set; }

    /// <summary>
    /// Customer phone number
    /// </summary>
    public string? CustomerPhone { get; set; }
}
