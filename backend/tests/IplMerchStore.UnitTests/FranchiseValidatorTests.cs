using IplMerchStore.Application.DTOs;
using IplMerchStore.Application.Validators;

namespace IplMerchStore.UnitTests;

/// <summary>
/// Unit tests for FranchiseValidator
/// </summary>
public class FranchiseValidatorTests
{
    [Fact]
    public void ValidateFranchiseInputDto_WithValidData_ShouldSucceed()
    {
        // Arrange
        var inputDto = new FranchiseInputDto
        {
            Name = "Test Franchise",
            ShortCode = "TF",
            PrimaryColor = "#FF0000",
            SecondaryColor = "#00FF00",
            LogoUrl = "https://example.com/logo.png"
        };

        // Act
        var (isValid, errors) = FranchiseValidator.ValidateFranchiseInputDto(inputDto);

        // Assert
        Assert.True(isValid);
        Assert.Empty(errors);
    }

    [Fact]
    public void ValidateFranchiseInputDto_WithEmptyName_ShouldFail()
    {
        // Arrange
        var inputDto = new FranchiseInputDto
        {
            Name = "",
            ShortCode = "TF",
            PrimaryColor = "#FF0000",
            SecondaryColor = "#00FF00"
        };

        // Act
        var (isValid, errors) = FranchiseValidator.ValidateFranchiseInputDto(inputDto);

        // Assert
        Assert.False(isValid);
        Assert.NotEmpty(errors);
        Assert.Contains("required", errors[0], StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ValidateFranchiseInputDto_WithNameExceedingMaxLength_ShouldFail()
    {
        // Arrange
        var inputDto = new FranchiseInputDto
        {
            Name = new string('a', 101),
            ShortCode = "TF",
            PrimaryColor = "#FF0000",
            SecondaryColor = "#00FF00"
        };

        // Act
        var (isValid, errors) = FranchiseValidator.ValidateFranchiseInputDto(inputDto);

        // Assert
        Assert.False(isValid);
        Assert.NotEmpty(errors);
    }

    [Fact]
    public void ValidateFranchiseInputDto_WithLowercaseShortCode_ShouldFail()
    {
        // Arrange
        var inputDto = new FranchiseInputDto
        {
            Name = "Test",
            ShortCode = "tf",
            PrimaryColor = "#FF0000",
            SecondaryColor = "#00FF00"
        };

        // Act
        var (isValid, errors) = FranchiseValidator.ValidateFranchiseInputDto(inputDto);

        // Assert
        Assert.False(isValid);
        Assert.NotEmpty(errors);
        Assert.Contains("uppercase", errors[0], StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ValidateFranchiseInputDto_WithInvalidHexColor_ShouldFail()
    {
        // Arrange
        var inputDto = new FranchiseInputDto
        {
            Name = "Test",
            ShortCode = "TF",
            PrimaryColor = "invalid",
            SecondaryColor = "#00FF00"
        };

        // Act
        var (isValid, errors) = FranchiseValidator.ValidateFranchiseInputDto(inputDto);

        // Assert
        Assert.False(isValid);
        Assert.NotEmpty(errors);
        Assert.Contains("hex color", errors[0], StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ValidateFranchiseInputDto_WithValidHexColorWithoutHash_ShouldFail()
    {
        // Arrange
        var inputDto = new FranchiseInputDto
        {
            Name = "Test",
            ShortCode = "TF",
            PrimaryColor = "FF0000",
            SecondaryColor = "#00FF00"
        };

        // Act
        var (isValid, errors) = FranchiseValidator.ValidateFranchiseInputDto(inputDto);

        // Assert
        // This should fail because our validator requires the # prefix
        Assert.False(isValid);
    }

    [Fact]
    public void ValidateFranchiseInputDto_WithEmptyShortCode_ShouldFail()
    {
        // Arrange
        var inputDto = new FranchiseInputDto
        {
            Name = "Test",
            ShortCode = "",
            PrimaryColor = "#FF0000",
            SecondaryColor = "#00FF00"
        };

        // Act
        var (isValid, errors) = FranchiseValidator.ValidateFranchiseInputDto(inputDto);

        // Assert
        Assert.False(isValid);
        Assert.NotEmpty(errors);
    }

    [Fact]
    public void ValidateFranchiseInputDto_WithShortCodeExceedingMaxLength_ShouldFail()
    {
        // Arrange
        var inputDto = new FranchiseInputDto
        {
            Name = "Test",
            ShortCode = "TOOLONGCODE",
            PrimaryColor = "#FF0000",
            SecondaryColor = "#00FF00"
        };

        // Act
        var (isValid, errors) = FranchiseValidator.ValidateFranchiseInputDto(inputDto);

        // Assert
        Assert.False(isValid);
        Assert.NotEmpty(errors);
    }

    [Fact]
    public void ValidateFranchiseInputDto_WithLongLogoUrl_ShouldFail()
    {
        // Arrange
        var inputDto = new FranchiseInputDto
        {
            Name = "Test",
            ShortCode = "TF",
            PrimaryColor = "#FF0000",
            SecondaryColor = "#00FF00",
            LogoUrl = "https://example.com/" + new string('a', 500)
        };

        // Act
        var (isValid, errors) = FranchiseValidator.ValidateFranchiseInputDto(inputDto);

        // Assert
        Assert.False(isValid);
        Assert.NotEmpty(errors);
    }
}
