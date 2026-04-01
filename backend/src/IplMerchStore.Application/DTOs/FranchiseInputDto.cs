namespace IplMerchStore.Application.DTOs;

public class FranchiseInputDto
{
    public required string Name { get; set; }
    public required string ShortCode { get; set; }
    public required string PrimaryColor { get; set; }
    public required string SecondaryColor { get; set; }
    public string? LogoUrl { get; set; }
}
