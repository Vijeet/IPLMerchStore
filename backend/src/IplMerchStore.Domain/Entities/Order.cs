using IplMerchStore.Domain.Common;
using IplMerchStore.Domain.Enums;

namespace IplMerchStore.Domain.Entities;

/// <summary>
/// Represents an order placed by a customer
/// </summary>
public class Order : BaseEntity
{
    public required string UserId { get; set; }
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public string? ShippingAddress { get; set; }
    public string? CustomerEmail { get; set; }
    public string? CustomerPhone { get; set; }

    // Navigation properties
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}
