using IplMerchStore.Application.DTOs;
using IplMerchStore.Application.Validators;
using Xunit;

namespace IplMerchStore.UnitTests;

/// <summary>
/// Unit tests for ProductValidator
/// </summary>
public class ProductValidatorTests
{
    [Fact]
    public void ValidateProductInputDto_WithValidData_ShouldSucceed()
    {
        // Arrange
        var inputDto = new ProductInputDto
        {
            Name = "CSK Premium Jersey",
            Description = "Official CSK cricket jersey",
            Price = 3499m,
            Currency = "INR",
            InventoryCount = 50,
            ProductType = 1,
            FranchiseId = 1,
            ImageUrl = "https://example.com/jersey.jpg",
            IsActive = true,
            SKU = "CSK-JERSEY-001"
        };

        // Act
        var (isValid, errors) = ProductValidator.ValidateProductInputDto(inputDto);

        // Assert
        Assert.True(isValid);
        Assert.Empty(errors);
    }

    [Fact]
    public void ValidateProductInputDto_WithEmptyName_ShouldFail()
    {
        // Arrange
        var inputDto = new ProductInputDto
        {
            Name = "",
            Description = "Official CSK cricket jersey",
            Price = 3499m,
            Currency = "INR",
            InventoryCount = 50,
            ProductType = 1,
            FranchiseId = 1,
            SKU = "CSK-JERSEY-001"
        };

        // Act
        var (isValid, errors) = ProductValidator.ValidateProductInputDto(inputDto);

        // Assert
        Assert.False(isValid);
        Assert.NotEmpty(errors);
        Assert.Contains("Product name is required", errors);
    }

    [Fact]
    public void ValidateProductInputDto_WithTooLongName_ShouldFail()
    {
        // Arrange
        var inputDto = new ProductInputDto
        {
            Name = new string('a', 201),
            Description = "Official CSK cricket jersey",
            Price = 3499m,
            Currency = "INR",
            InventoryCount = 50,
            ProductType = 1,
            FranchiseId = 1,
            SKU = "CSK-JERSEY-001"
        };

        // Act
        var (isValid, errors) = ProductValidator.ValidateProductInputDto(inputDto);

        // Assert
        Assert.False(isValid);
        Assert.Contains("Product name must not exceed 200 characters", errors);
    }

    [Fact]
    public void ValidateProductInputDto_WithZeroPrice_ShouldFail()
    {
        // Arrange
        var inputDto = new ProductInputDto
        {
            Name = "CSK Premium Jersey",
            Description = "Official CSK cricket jersey",
            Price = 0m,
            Currency = "INR",
            InventoryCount = 50,
            ProductType = 1,
            FranchiseId = 1,
            SKU = "CSK-JERSEY-001"
        };

        // Act
        var (isValid, errors) = ProductValidator.ValidateProductInputDto(inputDto);

        // Assert
        Assert.False(isValid);
        Assert.Contains("Product price must be greater than 0", errors);
    }

    [Fact]
    public void ValidateProductInputDto_WithNegativePrice_ShouldFail()
    {
        // Arrange
        var inputDto = new ProductInputDto
        {
            Name = "CSK Premium Jersey",
            Description = "Official CSK cricket jersey",
            Price = -100m,
            Currency = "INR",
            InventoryCount = 50,
            ProductType = 1,
            FranchiseId = 1,
            SKU = "CSK-JERSEY-001"
        };

        // Act
        var (isValid, errors) = ProductValidator.ValidateProductInputDto(inputDto);

        // Assert
        Assert.False(isValid);
        Assert.Contains("Product price must be greater than 0", errors);
    }

    [Fact]
    public void ValidateProductInputDto_WithInvalidCurrency_ShouldFail()
    {
        // Arrange
        var inputDto = new ProductInputDto
        {
            Name = "CSK Premium Jersey",
            Description = "Official CSK cricket jersey",
            Price = 3499m,
            Currency = "XYZ",
            InventoryCount = 50,
            ProductType = 1,
            FranchiseId = 1,
            SKU = "CSK-JERSEY-001"
        };

        // Act
        var (isValid, errors) = ProductValidator.ValidateProductInputDto(inputDto);

        // Assert
        Assert.False(isValid);
        Assert.Contains("Currency must be one of:", errors[0]);
    }

    [Fact]
    public void ValidateProductInputDto_WithNegativeInventoryCount_ShouldFail()
    {
        // Arrange
        var inputDto = new ProductInputDto
        {
            Name = "CSK Premium Jersey",
            Description = "Official CSK cricket jersey",
            Price = 3499m,
            Currency = "INR",
            InventoryCount = -5,
            ProductType = 1,
            FranchiseId = 1,
            SKU = "CSK-JERSEY-001"
        };

        // Act
        var (isValid, errors) = ProductValidator.ValidateProductInputDto(inputDto);

        // Assert
        Assert.False(isValid);
        Assert.Contains("Inventory count must be greater than or equal to 0", errors);
    }

    [Fact]
    public void ValidateProductInputDto_WithInvalidProductType_ShouldFail()
    {
        // Arrange
        var inputDto = new ProductInputDto
        {
            Name = "CSK Premium Jersey",
            Description = "Official CSK cricket jersey",
            Price = 3499m,
            Currency = "INR",
            InventoryCount = 50,
            ProductType = 99, // Invalid
            FranchiseId = 1,
            SKU = "CSK-JERSEY-001"
        };

        // Act
        var (isValid, errors) = ProductValidator.ValidateProductInputDto(inputDto);

        // Assert
        Assert.False(isValid);
        Assert.Contains("Invalid product type", errors);
    }

    [Fact]
    public void ValidateProductInputDto_WithInvalidFranchiseId_ShouldFail()
    {
        // Arrange
        var inputDto = new ProductInputDto
        {
            Name = "CSK Premium Jersey",
            Description = "Official CSK cricket jersey",
            Price = 3499m,
            Currency = "INR",
            InventoryCount = 50,
            ProductType = 1,
            FranchiseId = 0, // Invalid
            SKU = "CSK-JERSEY-001"
        };

        // Act
        var (isValid, errors) = ProductValidator.ValidateProductInputDto(inputDto);

        // Assert
        Assert.False(isValid);
        Assert.Contains("Valid franchise ID is required", errors);
    }

    [Fact]
    public void ValidateProductInputDto_WithEmptySKU_ShouldFail()
    {
        // Arrange
        var inputDto = new ProductInputDto
        {
            Name = "CSK Premium Jersey",
            Description = "Official CSK cricket jersey",
            Price = 3499m,
            Currency = "INR",
            InventoryCount = 50,
            ProductType = 1,
            FranchiseId = 1,
            SKU = ""
        };

        // Act
        var (isValid, errors) = ProductValidator.ValidateProductInputDto(inputDto);

        // Assert
        Assert.False(isValid);
        Assert.Contains("SKU (Stock Keeping Unit) is required", errors);
    }

    [Fact]
    public void ValidateProductInputDto_WithInvalidSKUCharacters_ShouldFail()
    {
        // Arrange
        var inputDto = new ProductInputDto
        {
            Name = "CSK Premium Jersey",
            Description = "Official CSK cricket jersey",
            Price = 3499m,
            Currency = "INR",
            InventoryCount = 50,
            ProductType = 1,
            FranchiseId = 1,
            SKU = "CSK@JERSEY#001" // Invalid characters
        };

        // Act
        var (isValid, errors) = ProductValidator.ValidateProductInputDto(inputDto);

        // Assert
        Assert.False(isValid);
        Assert.Contains("SKU must contain only alphanumeric characters, hyphens, and underscores", errors);
    }

    [Fact]
    public void ValidateProductInputDto_WithValidSKUWithHyphensAndUnderscores_ShouldSucceed()
    {
        // Arrange
        var inputDto = new ProductInputDto
        {
            Name = "CSK Premium Jersey",
            Description = "Official CSK cricket jersey",
            Price = 3499m,
            Currency = "INR",
            InventoryCount = 50,
            ProductType = 1,
            FranchiseId = 1,
            SKU = "CSK-JERSEY_001"
        };

        // Act
        var (isValid, errors) = ProductValidator.ValidateProductInputDto(inputDto);

        // Assert
        Assert.True(isValid);
        Assert.Empty(errors);
    }

    [Fact]
    public void ValidateProductInputDto_WithEmptyDescription_ShouldFail()
    {
        // Arrange
        var inputDto = new ProductInputDto
        {
            Name = "CSK Premium Jersey",
            Description = "",
            Price = 3499m,
            Currency = "INR",
            InventoryCount = 50,
            ProductType = 1,
            FranchiseId = 1,
            SKU = "CSK-JERSEY-001"
        };

        // Act
        var (isValid, errors) = ProductValidator.ValidateProductInputDto(inputDto);

        // Assert
        Assert.False(isValid);
        Assert.Contains("Product description is required", errors);
    }

    [Fact]
    public void ValidateProductInputDto_WithMultipleErrors_ShouldReturnAllErrors()
    {
        // Arrange
        var inputDto = new ProductInputDto
        {
            Name = "", // Empty
            Description = "", // Empty
            Price = -100m, // Negative
            Currency = "INVALID", // Invalid
            InventoryCount = -10, // Negative
            ProductType = 99, // Invalid
            FranchiseId = -1, // Invalid
            SKU = "" // Empty
        };

        // Act
        var (isValid, errors) = ProductValidator.ValidateProductInputDto(inputDto);

        // Assert
        Assert.False(isValid);
        Assert.NotEmpty(errors);
        Assert.True(errors.Count > 1); // Should have multiple errors
    }

    [Theory]
    [InlineData("USD")]
    [InlineData("EUR")]
    [InlineData("GBP")]
    [InlineData("INR")]
    public void ValidateProductInputDto_WithValidCurrencies_ShouldSucceed(string currency)
    {
        // Arrange
        var inputDto = new ProductInputDto
        {
            Name = "Test Product",
            Description = "Test Description",
            Price = 100m,
            Currency = currency,
            InventoryCount = 10,
            ProductType = 1,
            FranchiseId = 1,
            SKU = "TEST-001"
        };

        // Act
        var (isValid, errors) = ProductValidator.ValidateProductInputDto(inputDto);

        // Assert
        Assert.True(isValid);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(8)]
    public void ValidateProductInputDto_WithValidProductTypes_ShouldSucceed(int productType)
    {
        // Arrange
        var inputDto = new ProductInputDto
        {
            Name = "Test Product",
            Description = "Test Description",
            Price = 100m,
            Currency = "INR",
            InventoryCount = 10,
            ProductType = productType,
            FranchiseId = 1,
            SKU = "TEST-001"
        };

        // Act
        var (isValid, errors) = ProductValidator.ValidateProductInputDto(inputDto);

        // Assert
        Assert.True(isValid);
    }

    [Fact]
    public void ValidateProductInputDto_WithZeroInventoryCount_ShouldSucceed()
    {
        // Arrange
        var inputDto = new ProductInputDto
        {
            Name = "CSK Premium Jersey",
            Description = "Official CSK cricket jersey",
            Price = 3499m,
            Currency = "INR",
            InventoryCount = 0,
            ProductType = 1,
            FranchiseId = 1,
            SKU = "CSK-JERSEY-001"
        };

        // Act
        var (isValid, errors) = ProductValidator.ValidateProductInputDto(inputDto);

        // Assert
        Assert.True(isValid);
        Assert.Empty(errors);
    }
}
