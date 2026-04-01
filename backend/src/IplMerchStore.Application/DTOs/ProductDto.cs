namespace IplMerchStore.Application.DTOs;

public class ProductDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public int ProductType { get; set; }
    public int FranchiseId { get; set; }
    public string? FranchiseName { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; }
}
