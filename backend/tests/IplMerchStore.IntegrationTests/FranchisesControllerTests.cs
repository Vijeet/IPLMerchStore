using System.Net;
using System.Text;
using System.Text.Json;
using IplMerchStore.Application.DTOs;

namespace IplMerchStore.IntegrationTests;

/// <summary>
/// Integration tests for Franchises API endpoints
/// </summary>
public class FranchisesControllerTests : IClassFixture<IplMerchStoreWebApplicationFactory>
{
    private readonly IplMerchStoreWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public FranchisesControllerTests(IplMerchStoreWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetFranchises_ShouldReturnOkWithEmptyList()
    {
        // Act
        var response = await _client.GetAsync("/api/franchises");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.NotEmpty(content);
        Assert.Contains("Items", content);
    }

    [Fact]
    public async Task CreateFranchise_WithValidData_ShouldReturnCreated()
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

        var json = JsonSerializer.Serialize(inputDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/franchises", content);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Contains("Test Franchise", responseContent);
    }

    [Fact]
    public async Task CreateFranchise_WithInvalidData_ShouldReturnBadRequest()
    {
        // Arrange
        var inputDto = new FranchiseInputDto
        {
            Name = "", // Invalid
            ShortCode = "TF",
            PrimaryColor = "#FF0000",
            SecondaryColor = "#00FF00"
        };

        var json = JsonSerializer.Serialize(inputDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/franchises", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateFranchise_WithDuplicateName_ShouldReturnBadRequest()
    {
        // Arrange
        var firstDto = new FranchiseInputDto
        {
            Name = "Duplicate Test",
            ShortCode = "DT1",
            PrimaryColor = "#FF0000",
            SecondaryColor = "#00FF00"
        };

        var secondDto = new FranchiseInputDto
        {
            Name = "Duplicate Test", // Same name
            ShortCode = "DT2",
            PrimaryColor = "#0000FF",
            SecondaryColor = "#FFFF00"
        };

        var json1 = JsonSerializer.Serialize(firstDto);
        var content1 = new StringContent(json1, Encoding.UTF8, "application/json");

        var json2 = JsonSerializer.Serialize(secondDto);
        var content2 = new StringContent(json2, Encoding.UTF8, "application/json");

        // Act
        var response1 = await _client.PostAsync("/api/franchises", content1);
        var response2 = await _client.PostAsync("/api/franchises", content2);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response1.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, response2.StatusCode);
    }

    [Fact]
    public async Task CreateFranchise_WithDuplicateShortCode_ShouldReturnBadRequest()
    {
        // Arrange
        var firstDto = new FranchiseInputDto
        {
            Name = "First Franchise",
            ShortCode = "FF",
            PrimaryColor = "#FF0000",
            SecondaryColor = "#00FF00"
        };

        var secondDto = new FranchiseInputDto
        {
            Name = "Second Franchise",
            ShortCode = "FF", // Same short code
            PrimaryColor = "#0000FF",
            SecondaryColor = "#FFFF00"
        };

        var json1 = JsonSerializer.Serialize(firstDto);
        var content1 = new StringContent(json1, Encoding.UTF8, "application/json");

        var json2 = JsonSerializer.Serialize(secondDto);
        var content2 = new StringContent(json2, Encoding.UTF8, "application/json");

        // Act
        var response1 = await _client.PostAsync("/api/franchises", content1);
        var response2 = await _client.PostAsync("/api/franchises", content2);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response1.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, response2.StatusCode);
    }

    [Fact]
    public async Task GetFranchiseById_WithValidId_ShouldReturnOk()
    {
        // Arrange
        var inputDto = new FranchiseInputDto
        {
            Name = "Test Get By ID",
            ShortCode = "TGBI",
            PrimaryColor = "#FF0000",
            SecondaryColor = "#00FF00"
        };

        var json = JsonSerializer.Serialize(inputDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var createResponse = await _client.PostAsync("/api/franchises", content);
        var createdContent = await createResponse.Content.ReadAsStringAsync();
        var franchiseId = ExtractId(createdContent);

        // Act
        var response = await _client.GetAsync($"/api/franchises/{franchiseId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Contains("Test Get By ID", responseContent);
    }

    [Fact]
    public async Task GetFranchiseById_WithInvalidId_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/franchises/99999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateFranchise_WithValidData_ShouldReturnOk()
    {
        // Arrange
        var createInputDto = new FranchiseInputDto
        {
            Name = "Test Update",
            ShortCode = "TU",
            PrimaryColor = "#FF0000",
            SecondaryColor = "#00FF00"
        };

        var json = JsonSerializer.Serialize(createInputDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var createResponse = await _client.PostAsync("/api/franchises", content);
        var createdContent = await createResponse.Content.ReadAsStringAsync();
        var franchiseId = ExtractId(createdContent);

        var updateInputDto = new FranchiseInputDto
        {
            Name = "Updated Franchise",
            ShortCode = "UF",
            PrimaryColor = "#0000FF",
            SecondaryColor = "#FFFF00"
        };

        var updateJson = JsonSerializer.Serialize(updateInputDto);
        var updateContent = new StringContent(updateJson, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PutAsync($"/api/franchises/{franchiseId}", updateContent);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Contains("Updated Franchise", responseContent);
    }

    [Fact]
    public async Task UpdateFranchise_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var inputDto = new FranchiseInputDto
        {
            Name = "Test",
            ShortCode = "T",
            PrimaryColor = "#FF0000",
            SecondaryColor = "#00FF00"
        };

        var json = JsonSerializer.Serialize(inputDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PutAsync("/api/franchises/99999", content);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteFranchise_WithNoProducts_ShouldReturnOk()
    {
        // Arrange
        var inputDto = new FranchiseInputDto
        {
            Name = "Test Delete",
            ShortCode = "TD",
            PrimaryColor = "#FF0000",
            SecondaryColor = "#00FF00"
        };

        var json = JsonSerializer.Serialize(inputDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var createResponse = await _client.PostAsync("/api/franchises", content);
        var createdContent = await createResponse.Content.ReadAsStringAsync();
        var franchiseId = ExtractId(createdContent);

        // Act
        var response = await _client.DeleteAsync($"/api/franchises/{franchiseId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Verify it's deleted
        var getResponse = await _client.GetAsync($"/api/franchises/{franchiseId}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteFranchise_WithInvalidId_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.DeleteAsync("/api/franchises/99999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetFranchises_WithPagination_ShouldReturnPagedResult()
    {
        // Arrange - Create multiple franchises
        for (int i = 0; i < 3; i++)
        {
            var inputDto = new FranchiseInputDto
            {
                Name = $"Franchise {i}",
                ShortCode = $"F{i}",
                PrimaryColor = "#FF0000",
                SecondaryColor = "#00FF00"
            };

            var json = JsonSerializer.Serialize(inputDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            await _client.PostAsync("/api/franchises", content);
        }

        // Act
        var response = await _client.GetAsync("/api/franchises?pageNumber=1&pageSize=2");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Contains("PageNumber", responseContent);
        Assert.Contains("PageSize", responseContent);
    }

    private static int ExtractId(string json)
    {
        using var jsonDoc = JsonDocument.Parse(json);
        var root = jsonDoc.RootElement;
        if (root.TryGetProperty("data", out var data) && data.TryGetProperty("id", out var id))
        {
            return id.GetInt32();
        }
        throw new InvalidOperationException("Could not extract ID from response");
    }
}
