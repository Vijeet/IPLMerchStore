using IplMerchStore.Application.DTOs;

namespace IplMerchStore.Application.Validators;

/// <summary>
/// Validator for franchise DTOs
/// </summary>
public static class FranchiseValidator
{
    public static (bool IsValid, List<string> Errors) ValidateFranchiseInputDto(FranchiseInputDto dto)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(dto.Name))
            errors.Add("Franchise name is required");
        else if (dto.Name.Length > 100)
            errors.Add("Franchise name must not exceed 100 characters");

        if (string.IsNullOrWhiteSpace(dto.ShortCode))
            errors.Add("Short code is required");
        else
        {
            if (dto.ShortCode.Length > 10)
                errors.Add("Short code must not exceed 10 characters");
            if (!dto.ShortCode.Equals(dto.ShortCode.ToUpperInvariant()))
                errors.Add("Short code must be uppercase");
        }

        if (string.IsNullOrWhiteSpace(dto.PrimaryColor))
            errors.Add("Primary color is required");
        else if (!IsValidHexColor(dto.PrimaryColor))
            errors.Add("Primary color must be a valid hex color code (e.g., #FF5733)");

        if (string.IsNullOrWhiteSpace(dto.SecondaryColor))
            errors.Add("Secondary color is required");
        else if (!IsValidHexColor(dto.SecondaryColor))
            errors.Add("Secondary color must be a valid hex color code (e.g., #FF5733)");

        if (!string.IsNullOrWhiteSpace(dto.LogoUrl) && dto.LogoUrl.Length > 500)
            errors.Add("Logo URL must not exceed 500 characters");

        return (errors.Count == 0, errors);
    }

    private static bool IsValidHexColor(string color)
    {
        if (string.IsNullOrWhiteSpace(color))
            return false;

        // Require # prefix and check for valid 6-digit hex code
        if (!color.StartsWith("#"))
            return false;

        var hexPart = color[1..];
        return hexPart.Length == 6 && hexPart.All(c => "0123456789ABCDEFabcdef".Contains(c));
    }
}
