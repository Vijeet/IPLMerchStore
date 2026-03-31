namespace IplMerchStore.Domain.Common;

/// <summary>
/// Base entity for all domain objects
/// </summary>
public abstract class BaseEntity
{
    public required int Id { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}
