namespace IplMerchStore.Application.DTOs;

public class CartDto
{
    public int Id { get; set; }
    public string? UserId { get; set; }
    public decimal TotalPrice { get; set; }
    public IEnumerable<CartItemDto> Items { get; set; } = new List<CartItemDto>();
}
