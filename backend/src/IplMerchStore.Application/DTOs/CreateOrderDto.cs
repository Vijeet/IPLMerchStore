namespace IplMerchStore.Application.DTOs;

public class CreateOrderDto
{
    public string? ShippingAddress { get; set; }
    public string? CustomerEmail { get; set; }
    public string? CustomerPhone { get; set; }
}
