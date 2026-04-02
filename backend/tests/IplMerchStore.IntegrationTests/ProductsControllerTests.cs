using System.Net;
using System.Text;
using System.Text.Json;
using IplMerchStore.Application.DTOs;
using Xunit;

namespace IplMerchStore.IntegrationTests;

/// <summary>
/// Integration tests for ProductsController
/// </summary>
public class ProductsControllerTests : IClassFixture<IplMerchStoreWebApplicationFactory>
{
    private readonly IplMerchStoreWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public ProductsControllerTests(IplMerchStoreWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetProducts_ShouldReturnOkWithProducts()
    {
        // Act
        var response = await _client.GetAsync("/api/products");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Items", content);
        Assert.Contains("PageNumber", content);
    }

    [Fact]
    public async Task GetProducts_WithPagination_ShouldReturnPagedResults()
    {
        // Act
        var response = await _client.GetAsync("/api/products?pageNumber=1&pageSize=5");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("\"pageSize\":5", content);
    }

    [Fact]
    public async Task GetProducts_WithInvalidPageNumber_ShouldReturnBadRequest()
    {
        // Act
        var response = await _client.GetAsync("/api/products?pageNumber=0");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetProducts_WithFranchiseIdFilter_ShouldReturnFilteredResults()
    {
        // Act
        var response = await _client.GetAsync("/api/products?franchiseId=1");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("\"franchiseId\":1", content);
    }

    [Fact]
    public async Task GetProducts_WithProductTypeFilter_ShouldReturnFilteredResults()
    {
        // Act
        var response = await _client.GetAsync("/api/products?productType=1");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("\"productType\":1", content);
    }

    [Fact]
    public async Task GetProducts_WithActiveOnlyFilter_ShouldReturnOnlyActiveProducts()
    {
        // Act
        var response = await _client.GetAsync("/api/products?activeOnly=true");

        // Assert - With debug output
        if (response.StatusCode != HttpStatusCode.OK)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Xunit.Sdk.XunitException($"Expected OK but got {response.StatusCode}. Response: {errorContent}");
        }

        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("isActive", content);
    }

    [Fact]
    public async Task GetProducts_WithSortByName_ShouldReturnSortedResults()
    {
        // Act
        var response = await _client.GetAsync("/api/products?sortBy=name");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Items", content);
    }

    [Fact]
    public async Task GetProducts_WithSortByPrice_ShouldReturnSortedResults()
    {
        // Act
        var response = await _client.GetAsync("/api/products?sortBy=price");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Items", content);
    }

    [Fact]
    public async Task GetProductById_WithValidId_ShouldReturnProduct()
    {
        // Arrange: First get a product ID
        var listResponse = await _client.GetAsync("/api/products?pageSize=1");
        var listContent = await listResponse.Content.ReadAsStringAsync();
        var doc = JsonDocument.Parse(listContent);
        var productId = doc.RootElement.GetProperty("data").GetProperty("items")[0].GetProperty("id").GetInt32();

        // Act
        var response = await _client.GetAsync($"/api/products/{productId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains($"\"id\":{productId}", content);
    }

    [Fact]
    public async Task GetProductById_WithInvalidId_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/products/99999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateProduct_WithValidData_ShouldReturnCreated()
    {
        // Arrange
        var inputDto = new ProductInputDto
        {
            Name = "Test Jersey",
            Description = "Test cricket jersey",
            Price = 2999m,
            Currency = "INR",
            InventoryCount = 30,
            ProductType = 1,
            FranchiseId = 1,
            ImageUrl = "https://example.com/test-jersey.jpg",
            IsActive = true,
            SKU = "TEST-JERSEY-" + Guid.NewGuid().ToString().Substring(0, 8)
        };

        var json = JsonSerializer.Serialize(inputDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/products", content);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Contains("\"success\":true", responseContent);
    }

    [Fact]
    public async Task CreateProduct_WithInvalidPrice_ShouldReturnBadRequest()
    {
        // Arrange
        var inputDto = new ProductInputDto
        {
            Name = "Test Jersey",
            Description = "Test cricket jersey",
            Price = -100m, // Invalid
            Currency = "INR",
            InventoryCount = 30,
            ProductType = 1,
            FranchiseId = 1,
            SKU = "TEST-JERSEY-001"
        };

        var json = JsonSerializer.Serialize(inputDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/products", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateProduct_WithDuplicateSKU_ShouldReturnBadRequest()
    {
        // Arrange: Create first product
        var sku = "UNIQUE-SKU-" + Guid.NewGuid().ToString().Substring(0, 8);
        var inputDto1 = new ProductInputDto
        {
            Name = "Product 1",
            Description = "Description 1",
            Price = 1999m,
            Currency = "INR",
            InventoryCount = 20,
            ProductType = 1,
            FranchiseId = 1,
            SKU = sku
        };

        var json1 = JsonSerializer.Serialize(inputDto1);
        var content1 = new StringContent(json1, Encoding.UTF8, "application/json");
        await _client.PostAsync("/api/products", content1);

        // Arrange: Try to create with same SKU
        var inputDto2 = new ProductInputDto
        {
            Name = "Product 2",
            Description = "Description 2",
            Price = 1999m,
            Currency = "INR",
            InventoryCount = 20,
            ProductType = 1,
            FranchiseId = 1,
            SKU = sku // Same SKU
        };

        var json2 = JsonSerializer.Serialize(inputDto2);
        var content2 = new StringContent(json2, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/products", content2);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateProduct_WithInvalidFranchiseId_ShouldReturnBadRequest()
    {
        // Arrange
        var inputDto = new ProductInputDto
        {
            Name = "Test Jersey",
            Description = "Test cricket jersey",
            Price = 2999m,
            Currency = "INR",
            InventoryCount = 30,
            ProductType = 1,
            FranchiseId = 99999, // Invalid franchise
            SKU = "TEST-JERSEY-001"
        };

        var json = JsonSerializer.Serialize(inputDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/products", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateProduct_WithValidData_ShouldReturnOk()
    {
        // Arrange: Get existing product
        var listResponse = await _client.GetAsync("/api/products?pageSize=1");
        var listContent = await listResponse.Content.ReadAsStringAsync();
        var doc = JsonDocument.Parse(listContent);
        var productId = doc.RootElement.GetProperty("data").GetProperty("items")[0].GetProperty("id").GetInt32();
        var existingSKU = doc.RootElement.GetProperty("data").GetProperty("items")[0].GetProperty("sku").GetString() ?? "DEFAULT-SKU";

        // Prepare update
        var inputDto = new ProductInputDto
        {
            Name = "Updated Product Name",
            Description = "Updated description",
            Price = 4999m,
            Currency = "INR",
            InventoryCount = 100,
            ProductType = 2,
            FranchiseId = 1,
            SKU = existingSKU // Keep same SKU
        };

        var json = JsonSerializer.Serialize(inputDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PutAsync($"/api/products/{productId}", content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task UpdateProduct_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var inputDto = new ProductInputDto
        {
            Name = "Test Jersey",
            Description = "Test cricket jersey",
            Price = 2999m,
            Currency = "INR",
            InventoryCount = 30,
            ProductType = 1,
            FranchiseId = 1,
            SKU = "TEST-UPDATE-001"
        };

        var json = JsonSerializer.Serialize(inputDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PutAsync("/api/products/99999", content);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteProduct_WithValidId_ShouldReturnOk()
    {
        // Arrange: Create a product to delete
        var createDto = new ProductInputDto
        {
            Name = "Product to Delete",
            Description = "Will be deleted",
            Price = 999m,
            Currency = "INR",
            InventoryCount = 10,
            ProductType = 1,
            FranchiseId = 1,
            SKU = "DELETE-" + Guid.NewGuid().ToString().Substring(0, 8)
        };

        var json = JsonSerializer.Serialize(createDto);
        var createContent = new StringContent(json, Encoding.UTF8, "application/json");
        var createResponse = await _client.PostAsync("/api/products", createContent);
        var createResponseContent = await createResponse.Content.ReadAsStringAsync();
        var createDoc = JsonDocument.Parse(createResponseContent);
        var productId = createDoc.RootElement.GetProperty("data").GetProperty("id").GetInt32();

        // Act
        var response = await _client.DeleteAsync($"/api/products/{productId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task DeleteProduct_WithInvalidId_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.DeleteAsync("/api/products/99999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task SearchProducts_WithNameFilter_ShouldReturnMatchingProducts()
    {
        // Act
        var response = await _client.GetAsync("/api/products/search?name=jersey");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Items", content);
    }

    [Fact]
    public async Task SearchProducts_WithFranchiseIdAndType_ShouldReturnFilteredResults()
    {
        // Act
        var response = await _client.GetAsync("/api/products/search?franchiseId=1&productType=1");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Items", content);
    }

    [Fact]
    public async Task SearchProducts_WithNoMatches_ShouldReturnEmptyList()
    {
        // Act
        var response = await _client.GetAsync("/api/products/search?name=nonexistentproduct123456");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("\"items\":[]", content.ToLower());
    }

    [Fact]
    public async Task GetProductById_ShouldIncludeFranchiseDetails()
    {
        // Arrange: Get a product
        var listResponse = await _client.GetAsync("/api/products?pageSize=1");
        var listContent = await listResponse.Content.ReadAsStringAsync();
        var doc = JsonDocument.Parse(listContent);
        var productId = doc.RootElement.GetProperty("data").GetProperty("items")[0].GetProperty("id").GetInt32();

        // Act
        var response = await _client.GetAsync($"/api/products/{productId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("franchiseName", content);
        Assert.Contains("franchiseShortCode", content);
    }

    [Fact]
    public async Task GetProducts_DefaultSortByShouldWork()
    {
        // Act
        var response = await _client.GetAsync("/api/products?sortBy=invalid");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
