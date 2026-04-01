using IplMerchStore.Domain.Common;

namespace IplMerchStore.Domain.Entities;

/// <summary>
/// Represents an IPL franchise
/// </summary>
public class Franchise : BaseEntity
{
    public required string Name { get; set; }
    public string? ShortCode { get; set; }
    public string? City { get; set; }
    public string? LogoUrl { get; set; }
    public string? Description { get; set; }

    // Navigation properties
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
