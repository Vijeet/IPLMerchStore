using IplMerchStore.Application.DTOs;

namespace IplMerchStore.Application.Validators;

/// <summary>
/// Validator for product DTOs
/// </summary>
public static class ProductValidator
{
    private static readonly string[] ValidCurrencies = { "INR", "USD", "EUR", "GBP" };
    private static readonly string[] ValidProductTypes = { "Jersey", "Cap", "Flag", "AutographedPhoto", "Mug", "Hoodie", "Keychain", "Other" };

    public static (bool IsValid, List<string> Errors) ValidateProductInputDto(ProductInputDto dto)
    {
        var errors = new List<string>();

        // Name validation
        if (string.IsNullOrWhiteSpace(dto.Name))
            errors.Add("Product name is required");
        else if (dto.Name.Length > 200)
            errors.Add("Product name must not exceed 200 characters");

        // Description validation
        if (string.IsNullOrWhiteSpace(dto.Description))
            errors.Add("Product description is required");
        else if (dto.Description.Length > 2000)
            errors.Add("Product description must not exceed 2000 characters");

        // Price validation
        if (dto.Price <= 0)
            errors.Add("Product price must be greater than 0");

        // Currency validation
        if (string.IsNullOrWhiteSpace(dto.Currency))
            errors.Add("Currency is required");
        else if (!ValidCurrencies.Contains(dto.Currency.ToUpperInvariant()))
            errors.Add($"Currency must be one of: {string.Join(", ", ValidCurrencies)}");

        // InventoryCount validation
        if (dto.InventoryCount < 0)
            errors.Add("Inventory count must be greater than or equal to 0");

        // ProductType validation
        if (dto.ProductType < 1 || dto.ProductType > 8)
            errors.Add("Invalid product type. Must be between 1 and 8");

        // FranchiseId validation
        if (dto.FranchiseId <= 0)
            errors.Add("Valid franchise ID is required");

        // SKU validation
        if (string.IsNullOrWhiteSpace(dto.SKU))
            errors.Add("SKU (Stock Keeping Unit) is required");
        else if (dto.SKU.Length > 100)
            errors.Add("SKU must not exceed 100 characters");
        else if (!IsValidSKU(dto.SKU))
            errors.Add("SKU must contain only alphanumeric characters, hyphens, and underscores");

        // ImageUrl validation
        if (!string.IsNullOrWhiteSpace(dto.ImageUrl) && dto.ImageUrl.Length > 500)
            errors.Add("Image URL must not exceed 500 characters");

        return (errors.Count == 0, errors);
    }

    private static bool IsValidSKU(string sku)
    {
        return sku.All(c => char.IsLetterOrDigit(c) || c == '-' || c == '_');
    }
}
