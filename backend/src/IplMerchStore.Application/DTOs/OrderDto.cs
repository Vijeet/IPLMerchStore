namespace IplMerchStore.Application.DTOs;

public class OrderDto
{
    public int Id { get; set; }
    public string? UserId { get; set; }
    public decimal TotalAmount { get; set; }
    public int Status { get; set; }
    public string? ShippingAddress { get; set; }
    public string? CustomerEmail { get; set; }
    public string? CustomerPhone { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public IEnumerable<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();
}
