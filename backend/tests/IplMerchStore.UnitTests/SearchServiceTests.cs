using IplMerchStore.Application.DTOs;
using IplMerchStore.Domain.Entities;
using IplMerchStore.Domain.Enums;
using IplMerchStore.Infrastructure.Persistence;
using IplMerchStore.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace IplMerchStore.UnitTests;

/// <summary>
/// Unit tests for SearchService
/// Tests search filtering, partial text matching, pagination, and suggestions
/// </summary>
public class SearchServiceTests
{
    private readonly DbContextOptions<AppDbContext> _dbContextOptions;
    private readonly Mock<ILogger<SearchService>> _mockLogger;

    public SearchServiceTests()
    {
        // Use in-memory database for testing
        _dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
            .Options;

        _mockLogger = new Mock<ILogger<SearchService>>();
    }

    private AppDbContext CreateDbContext()
    {
        return new AppDbContext(_dbContextOptions);
    }

    private void SeedTestData(AppDbContext dbContext)
    {
        // Clear existing data
        dbContext.Products.RemoveRange(dbContext.Products);
        dbContext.Franchises.RemoveRange(dbContext.Franchises);

        // Create test franchises
        var franchises = new[]
        {
            new Franchise { Id = 1, Name = "Chennai Super Kings", ShortCode = "CSK", PrimaryColor = "#FFFF00", SecondaryColor = "#000000" },
            new Franchise { Id = 2, Name = "Mumbai Indians", ShortCode = "MI", PrimaryColor = "#003366", SecondaryColor = "#FFFFFF" },
            new Franchise { Id = 3, Name = "Royal Challengers Bangalore", ShortCode = "RCB", PrimaryColor = "#EC143C", SecondaryColor = "#000000" }
        };

        dbContext.Franchises.AddRange(franchises);

        // Create test products
        var products = new[]
        {
            new Product
            {
                Id = 1,
                Name = "CSK Premium Jersey",
                Description = "Official Chennai Super Kings cricket jersey in yellow and navy colors",
                Price = 3499m,
                Currency = "INR",
                InventoryCount = 50,
                ProductType = ProductType.Jersey,
                FranchiseId = 1,
                ImageUrl = "https://example.com/csk-jersey.jpg",
                IsActive = true,
                SKU = "CSK-JERSEY-001",
                CreatedAtUtc = DateTime.UtcNow.AddDays(-10)
            },
            new Product
            {
                Id = 2,
                Name = "CSK Cricket Cap",
                Description = "Yellow cricket cap with CSK logo",
                Price = 599m,
                Currency = "INR",
                InventoryCount = 100,
                ProductType = ProductType.Cap,
                FranchiseId = 1,
                ImageUrl = "https://example.com/csk-cap.jpg",
                IsActive = true,
                SKU = "CSK-CAP-001",
                CreatedAtUtc = DateTime.UtcNow.AddDays(-5)
            },
            new Product
            {
                Id = 3,
                Name = "MI Team Jersey",
                Description = "Official Mumbai Indians cricket jersey in blue and orange",
                Price = 3499m,
                Currency = "INR",
                InventoryCount = 75,
                ProductType = ProductType.Jersey,
                FranchiseId = 2,
                ImageUrl = "https://example.com/mi-jersey.jpg",
                IsActive = true,
                SKU = "MI-JERSEY-001",
                CreatedAtUtc = DateTime.UtcNow.AddDays(-3)
            },
            new Product
            {
                Id = 4,
                Name = "RCB Virat Edition Jersey",
                Description = "Special edition RCB jersey with Virat Kohli signature",
                Price = 4999m,
                Currency = "INR",
                InventoryCount = 25,
                ProductType = ProductType.Jersey,
                FranchiseId = 3,
                ImageUrl = "https://example.com/rcb-virat-jersey.jpg",
                IsActive = true,
                SKU = "RCB-VIRAT-JERSEY-001",
                CreatedAtUtc = DateTime.UtcNow.AddDays(-2)
            },
            new Product
            {
                Id = 5,
                Name = "CSK Autographed Photo",
                Description = "Team autographed photo from CSK championship win",
                Price = 2999m,
                Currency = "INR",
                InventoryCount = 10,
                ProductType = ProductType.AutographedPhoto,
                FranchiseId = 1,
                ImageUrl = "https://example.com/csk-autograph.jpg",
                IsActive = true,
                SKU = "CSK-AUTOG-001",
                CreatedAtUtc = DateTime.UtcNow.AddDays(-1)
            },
            new Product
            {
                Id = 6,
                Name = "Cricket Hoodie",
                Description = "Generic cricket hoodie that mentions CSK and cricket",
                Price = 1999m,
                Currency = "INR",
                InventoryCount = 0,
                ProductType = ProductType.Hoodie,
                FranchiseId = 1,
                ImageUrl = "https://example.com/hoodie.jpg",
                IsActive = false, // Inactive product - should not appear in search
                SKU = "GENERIC-HOODIE-001",
                CreatedAtUtc = DateTime.UtcNow
            }
        };

        dbContext.Products.AddRange(products);
        dbContext.SaveChanges();
    }

    #region SearchProductsAsync Tests

    [Fact]
    public async Task SearchProductsAsync_WithoutQuery_ShouldReturnAllActiveProducts()
    {
        // Arrange
        using var dbContext = CreateDbContext();
        SeedTestData(dbContext);
        var service = new SearchService(dbContext, _mockLogger.Object);

        // Act
        var result = await service.SearchProductsAsync();

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(5, result.Data.TotalCount); // 5 active products (6th is inactive)
        Assert.Equal(5, result.Data.Items.Count());
    }

    [Fact]
    public async Task SearchProductsAsync_WithQueryMatchingName_ShouldReturnRelevantProducts()
    {
        // Arrange
        using var dbContext = CreateDbContext();
        SeedTestData(dbContext);
        var service = new SearchService(dbContext, _mockLogger.Object);

        // Act
        var result = await service.SearchProductsAsync(query: "Jersey");

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(4, result.Data.TotalCount); // All 4 jerseys
        Assert.All(result.Data.Items, product =>
        {
            Assert.Contains("Jersey", product.Name, StringComparison.OrdinalIgnoreCase);
        });
    }

    [Fact]
    public async Task SearchProductsAsync_WithPartialQueryMatch_ShouldReturnMatchingProducts()
    {
        // Arrange
        using var dbContext = CreateDbContext();
        SeedTestData(dbContext);
        var service = new SearchService(dbContext, _mockLogger.Object);

        // Act
        var result = await service.SearchProductsAsync(query: "jer");

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        // Should match "Jersey" products (case-insensitive)
        Assert.True(result.Data.TotalCount >= 3);
    }

    [Fact]
    public async Task SearchProductsAsync_WithQueryMatchingDescription_ShouldReturnProducts()
    {
        // Arrange
        using var dbContext = CreateDbContext();
        SeedTestData(dbContext);
        var service = new SearchService(dbContext, _mockLogger.Object);

        // Act
        var result = await service.SearchProductsAsync(query: "blue");

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        // MI Jersey has "blue" in description
        Assert.True(result.Data.TotalCount >= 1);
    }

    [Fact]
    public async Task SearchProductsAsync_WithFranchiseFilter_ShouldReturnOnlyMatchingFranchiseProducts()
    {
        // Arrange
        using var dbContext = CreateDbContext();
        SeedTestData(dbContext);
        var service = new SearchService(dbContext, _mockLogger.Object);

        // Act
        var result = await service.SearchProductsAsync(franchiseId: 1); // CSK

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(4, result.Data.TotalCount); // CSK has 4 active products
        Assert.All(result.Data.Items, product =>
        {
            Assert.Equal(1, product.FranchiseId);
        });
    }

    [Fact]
    public async Task SearchProductsAsync_WithProductTypeFilter_ShouldReturnOnlyMatchingType()
    {
        // Arrange
        using var dbContext = CreateDbContext();
        SeedTestData(dbContext);
        var service = new SearchService(dbContext, _mockLogger.Object);

        // Act
        var result = await service.SearchProductsAsync(productType: (int)ProductType.Jersey);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(4, result.Data.TotalCount); // 4 jerseys
        Assert.All(result.Data.Items, product =>
        {
            Assert.Equal((int)ProductType.Jersey, product.ProductType);
        });
    }

    [Fact]
    public async Task SearchProductsAsync_WithMultipleFilters_ShouldReturnFilteredResults()
    {
        // Arrange
        using var dbContext = CreateDbContext();
        SeedTestData(dbContext);
        var service = new SearchService(dbContext, _mockLogger.Object);

        // Act
        var result = await service.SearchProductsAsync(
            query: "Jersey",
            franchiseId: 1,
            productType: (int)ProductType.Jersey);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Single(result.Data.Items); // Only CSK Jersey
        var product = result.Data.Items.First();
        Assert.Equal("CSK Premium Jersey", product.Name);
        Assert.Equal(1, product.FranchiseId);
    }

    [Fact]
    public async Task SearchProductsAsync_WithPagination_ShouldReturnPagedResults()
    {
        // Arrange
        using var dbContext = CreateDbContext();
        SeedTestData(dbContext);
        var service = new SearchService(dbContext, _mockLogger.Object);

        // Act
        var result1 = await service.SearchProductsAsync(pageNumber: 1, pageSize: 2);
        var result2 = await service.SearchProductsAsync(pageNumber: 2, pageSize: 2);

        // Assert
        Assert.True(result1.Success);
        Assert.True(result2.Success);
        Assert.Equal(2, result1.Data.Items.Count());
        Assert.Equal(2, result2.Data.Items.Count());
        Assert.NotEqual(result1.Data.Items.First().Id, result2.Data.Items.First().Id);
        Assert.Equal(5, result1.Data.TotalPages); // 5 items / 2 per page
    }

    [Fact]
    public async Task SearchProductsAsync_WithInvalidPageSize_ShouldCapAtMaximum()
    {
        // Arrange
        using var dbContext = CreateDbContext();
        SeedTestData(dbContext);
        var service = new SearchService(dbContext, _mockLogger.Object);

        // Act
        var result = await service.SearchProductsAsync(pageSize: 500); // Exceeds max of 100

        // Assert
        Assert.True(result.Success);
        Assert.Equal(100, result.Data.PageSize);
    }

    [Fact]
    public async Task SearchProductsAsync_WithNoMatches_ShouldReturnEmptyResults()
    {
        // Arrange
        using var dbContext = CreateDbContext();
        SeedTestData(dbContext);
        var service = new SearchService(dbContext, _mockLogger.Object);

        // Act
        var result = await service.SearchProductsAsync(query: "NonexistentProduct");

        // Assert
        Assert.True(result.Success);
        Assert.Empty(result.Data.Items);
        Assert.Equal(0, result.Data.TotalCount);
    }

    [Fact]
    public async Task SearchProductsAsync_ShouldSortByRelevanceThenName()
    {
        // Arrange
        using var dbContext = CreateDbContext();
        SeedTestData(dbContext);
        var service = new SearchService(dbContext, _mockLogger.Object);

        // Act
        var result = await service.SearchProductsAsync(query: "Jersey");

        // Assert
        Assert.True(result.Success);
        var items = result.Data.Items.ToList();
        
        // First item should have highest relevance (name starts with Jersey)
        Assert.True(items[0].RelevanceScore >= 90);
        
        // Items should be sorted by relevance score descending
        for (int i = 1; i < items.Count; i++)
        {
            Assert.True(items[i - 1].RelevanceScore >= items[i].RelevanceScore);
        }
    }

    [Fact]
    public async Task SearchProductsAsync_ShouldExcludeInactiveProducts()
    {
        // Arrange
        using var dbContext = CreateDbContext();
        SeedTestData(dbContext);
        var service = new SearchService(dbContext, _mockLogger.Object);

        // Act - search for "Cricket" which matches the inactive hoodie
        var result = await service.SearchProductsAsync(query: "Cricket");

        // Assert
        Assert.True(result.Success);
        Assert.All(result.Data.Items, product =>
        {
            Assert.True(product.IsActive);
        });
    }

    #endregion

    #region GetSearchSuggestionsAsync Tests

    [Fact]
    public async Task GetSearchSuggestionsAsync_WithoutQuery_ShouldReturnProductNames()
    {
        // Arrange
        using var dbContext = CreateDbContext();
        SeedTestData(dbContext);
        var service = new SearchService(dbContext, _mockLogger.Object);

        // Act
        var result = await service.GetSearchSuggestionsAsync(limit: 5);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.NotEmpty(result.Data);
        Assert.True(result.Data.Count() <= 5);
    }

    [Fact]
    public async Task GetSearchSuggestionsAsync_WithQuery_ShouldReturnMatchingSuggestions()
    {
        // Arrange
        using var dbContext = CreateDbContext();
        SeedTestData(dbContext);
        var service = new SearchService(dbContext, _mockLogger.Object);

        // Act
        var result = await service.GetSearchSuggestionsAsync(query: "Jersey", limit: 10);

        // Assert
        Assert.True(result.Success);
        var suggestions = result.Data.ToList();
        Assert.NotEmpty(suggestions);
        Assert.All(suggestions, suggestion =>
        {
            Assert.Contains("Jersey", suggestion);
        });
    }

    [Fact]
    public async Task GetSearchSuggestionsAsync_WithFranchiseFilter_ShouldReturnOnlyMatchingFranchiseSuggestions()
    {
        // Arrange
        using var dbContext = CreateDbContext();
        SeedTestData(dbContext);
        var service = new SearchService(dbContext, _mockLogger.Object);

        // Act
        var result = await service.GetSearchSuggestionsAsync(franchiseId: 1, limit: 10);

        // Assert
        Assert.True(result.Success);
        var suggestions = result.Data.ToList();
        Assert.NotEmpty(suggestions);
        Assert.All(suggestions, suggestion =>
        {
            Assert.Contains("CSK", suggestion); // CSK is franchise shortcode for ID 1
        });
    }

    [Fact]
    public async Task GetSearchSuggestionsAsync_WithInvalidLimit_ShouldCapAtMaximum()
    {
        // Arrange
        using var dbContext = CreateDbContext();
        SeedTestData(dbContext);
        var service = new SearchService(dbContext, _mockLogger.Object);

        // Act
        var result = await service.GetSearchSuggestionsAsync(limit: 200);

        // Assert
        Assert.True(result.Success);
        Assert.True(result.Data.Count() <= 50); // Max limit should be 50
    }

    [Fact]
    public async Task GetSearchSuggestionsAsync_ShouldIncludeFranchiseCode()
    {
        // Arrange
        using var dbContext = CreateDbContext();
        SeedTestData(dbContext);
        var service = new SearchService(dbContext, _mockLogger.Object);

        // Act
        var result = await service.GetSearchSuggestionsAsync(limit: 10);

        // Assert
        Assert.True(result.Success);
        var suggestions = result.Data.ToList();
        Assert.All(suggestions, suggestion =>
        {
            Assert.Contains("(", suggestion); // Should contain franchise code in parentheses
            Assert.Contains(")", suggestion);
        });
    }

    #endregion

    [Fact]
    public async Task SearchProductsAsync_OnException_ShouldReturnFailureResult()
    {
        // Arrange - Use null to cause exception
        var mockDbContext = new Mock<AppDbContext>();
        mockDbContext.Setup(x => x.Products).Throws(new Exception("Database error"));

        var service = new SearchService(mockDbContext.Object, _mockLogger.Object);

        // Act
        var result = await service.SearchProductsAsync("test");

        // Assert
        Assert.False(result.Success);
        Assert.NotNull(result.Message);
        Assert.Contains("Failed to search products", result.Message);
    }
}
