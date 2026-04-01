using IplMerchStore.Domain.Common;

namespace IplMerchStore.Domain.Entities;

/// <summary>
/// Represents an IPL franchise
/// </summary>
public class Franchise : BaseEntity
{
    public required string Name { get; set; }
    public required string ShortCode { get; set; }
    public required string PrimaryColor { get; set; }
    public required string SecondaryColor { get; set; }
    public string? LogoUrl { get; set; }

    // Navigation properties
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
