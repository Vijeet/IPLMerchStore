namespace IplMerchStore.Application.DTOs;

public class FranchiseDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? ShortCode { get; set; }
    public string? PrimaryColor { get; set; }
    public string? SecondaryColor { get; set; }
    public string? LogoUrl { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}
