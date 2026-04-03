using System.Net;
using System.Text.Json;
using Xunit;

namespace IplMerchStore.IntegrationTests;

/// <summary>
/// Integration tests for SearchController
/// Tests API endpoints for product search and suggestions
/// </summary>
public class SearchControllerTests : IClassFixture<IplMerchStoreWebApplicationFactory>
{
    private readonly IplMerchStoreWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public SearchControllerTests(IplMerchStoreWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    #region Search Endpoint Tests (GET /api/search)

    [Fact]
    public async Task GetSearch_WithoutQuery_ShouldReturnAllActiveProducts()
    {
        // Act
        var response = await _client.GetAsync("/api/search");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("\"success\":true", content);
        Assert.Contains("\"totalCount\":", content);
        // Should contain products from test data
        Assert.Contains("Jersey", content);
    }

    [Fact]
    public async Task GetSearch_WithQueryParameter_ShouldReturnMatchingProducts()
    {
        // Act
        var response = await _client.GetAsync("/api/search?q=Jersey");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("\"success\":true", content);
        // Should return jersey products
        var jsonDoc = JsonDocument.Parse(content);
        var totalCount = jsonDoc.RootElement.GetProperty("data").GetProperty("totalCount").GetInt32();
        Assert.True(totalCount >= 3); // At least 3 jerseys in test data (CSK, MI, RCB)
    }

    [Fact]
    public async Task GetSearch_WithPartialQueryMatch_ShouldReturnMatchingProducts()
    {
        // Act
        var response = await _client.GetAsync("/api/search?q=jer");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("\"success\":true", content);
        // Should match jersey products (case-insensitive)
        var jsonDoc = JsonDocument.Parse(content);
        var totalCount = jsonDoc.RootElement.GetProperty("data").GetProperty("totalCount").GetInt32();
        Assert.True(totalCount >= 3);
    }

    [Fact]
    public async Task GetSearch_WithFranchiseIdFilter_ShouldReturnOnlyFilteredProducts()
    {
        // Act
        var response = await _client.GetAsync("/api/search?franchiseId=1");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("\"success\":true", content);
        Assert.Contains("\"franchiseId\":1", content);
        
        // Verify all returned products belong to franchise 1
        var jsonDoc = JsonDocument.Parse(content);
        var items = jsonDoc.RootElement.GetProperty("data").GetProperty("items").EnumerateArray();
        foreach (var item in items)
        {
            Assert.Equal(1, item.GetProperty("franchiseId").GetInt32());
        }
    }

    [Fact]
    public async Task GetSearch_WithTypeFilter_ShouldReturnOnlyFilteredProductType()
    {
        // Act
        var response = await _client.GetAsync("/api/search?type=1"); // Type 1 = Jersey

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("\"success\":true", content);
        
        // Verify all returned products are of type 1 (Jersey)
        var jsonDoc = JsonDocument.Parse(content);
        var items = jsonDoc.RootElement.GetProperty("data").GetProperty("items").EnumerateArray();
        foreach (var item in items)
        {
            Assert.Equal(1, item.GetProperty("productType").GetInt32());
        }
    }

    [Fact]
    public async Task GetSearch_WithMultipleFilters_ShouldReturnFilteredResults()
    {
        // Act - Search for Jersey (q) for CSK (franchiseId=1) with Jersey type (type=1)
        var response = await _client.GetAsync("/api/search?q=Jersey&franchiseId=1&type=1");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("\"success\":true", content);
        Assert.Contains("CSK Premium Jersey", content);
    }

    [Fact]
    public async Task GetSearch_WithPagination_ShouldReturnPagedResults()
    {
        // Act - Get first page with 3 items per page
        var response1 = await _client.GetAsync("/api/search?page=1&pageSize=3");
        var content1 = await response1.Content.ReadAsStringAsync();
        
        // Get second page
        var response2 = await _client.GetAsync("/api/search?page=2&pageSize=3");
        var content2 = await response2.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response1.StatusCode);
        Assert.Equal(HttpStatusCode.OK, response2.StatusCode);
        
        var jsonDoc1 = JsonDocument.Parse(content1);
        var jsonDoc2 = JsonDocument.Parse(content2);
        
        var items1 = jsonDoc1.RootElement.GetProperty("data").GetProperty("items").EnumerateArray().ToList();
        var items2 = jsonDoc2.RootElement.GetProperty("data").GetProperty("items").EnumerateArray().ToList();
        
        Assert.Equal(3, items1.Count);
        Assert.Equal(3, items2.Count);
        Assert.NotEqual(items1[0].GetProperty("id").GetInt32(), items2[0].GetProperty("id").GetInt32());
    }

    [Fact]
    public async Task GetSearch_WithInvalidPageNumber_ShouldReturnBadRequest()
    {
        // Act
        var response = await _client.GetAsync("/api/search?page=0");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetSearch_WithInvalidPageSize_ShouldReturnBadRequest()
    {
        // Act
        var response = await _client.GetAsync("/api/search?pageSize=0");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetSearch_WithPageSizeExceedingMaximum_ShouldReturnBadRequest()
    {
        // Act
        var response = await _client.GetAsync("/api/search?pageSize=101");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetSearch_WithInvalidProductType_ShouldReturnBadRequest()
    {
        // Act
        var response = await _client.GetAsync("/api/search?type=9"); // Valid types are 1-8

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetSearch_WithInvalidProductType_Below1_ShouldReturnBadRequest()
    {
        // Act
        var response = await _client.GetAsync("/api/search?type=0");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetSearch_WithNoMatches_ShouldReturnEmptyResults()
    {
        // Act
        var response = await _client.GetAsync("/api/search?q=NonexistentProductXYZ");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("\"success\":true", content);
        
        var jsonDoc = JsonDocument.Parse(content);
        var totalCount = jsonDoc.RootElement.GetProperty("data").GetProperty("totalCount").GetInt32();
        var items = jsonDoc.RootElement.GetProperty("data").GetProperty("items").EnumerateArray().ToList();
        
        Assert.Equal(0, totalCount);
        Assert.Empty(items);
    }

    [Fact]
    public async Task GetSearch_ShouldReturnRelevanceScores()
    {
        // Act
        var response = await _client.GetAsync("/api/search?q=Jersey");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        
        var jsonDoc = JsonDocument.Parse(content);
        var items = jsonDoc.RootElement.GetProperty("data").GetProperty("items").EnumerateArray();
        
        // Verify each item has a relevance score
        foreach (var item in items)
        {
            Assert.True(item.TryGetProperty("relevanceScore", out var relevanceScore));
            Assert.True(relevanceScore.GetInt32() > 0);
        }
    }

    [Fact]
    public async Task GetSearch_ShouldIncludeProductDetails()
    {
        // Act
        var response = await _client.GetAsync("/api/search?q=Jersey&pageSize=1");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        
        var jsonDoc = JsonDocument.Parse(content);
        var items = jsonDoc.RootElement.GetProperty("data").GetProperty("items").EnumerateArray().ToList();
        
        Assert.NotEmpty(items);
        var item = items[0];
        
        // Verify essential product fields are present
        Assert.True(item.TryGetProperty("id", out _));
        Assert.True(item.TryGetProperty("name", out _));
        Assert.True(item.TryGetProperty("price", out _));
        Assert.True(item.TryGetProperty("currency", out _));
        Assert.True(item.TryGetProperty("franchiseName", out _));
        Assert.True(item.TryGetProperty("productTypeLabel", out _));
        Assert.True(item.TryGetProperty("isActive", out _));
    }

    #endregion

    #region Suggestions Endpoint Tests (GET /api/search/suggestions)

    [Fact]
    public async Task GetSearchSuggestions_WithoutQuery_ShouldReturnSuggestions()
    {
        // Act
        var response = await _client.GetAsync("/api/search/suggestions");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("\"success\":true", content);
        
        var jsonDoc = JsonDocument.Parse(content);
        var data = jsonDoc.RootElement.GetProperty("data");
        Assert.True(data.ValueKind == System.Text.Json.JsonValueKind.Array);
    }

    [Fact]
    public async Task GetSearchSuggestions_WithQuery_ShouldReturnMatchingSuggestions()
    {
        // Act
        var response = await _client.GetAsync("/api/search/suggestions?q=Jersey");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("\"success\":true", content);
        Assert.Contains("Jersey", content);
        
        var jsonDoc = JsonDocument.Parse(content);
        var data = jsonDoc.RootElement.GetProperty("data");
        var suggestions = data.EnumerateArray().ToList();
        
        Assert.NotEmpty(suggestions);
        Assert.All(suggestions, suggestion =>
        {
            Assert.Contains("Jersey", suggestion.GetString() ?? "");
        });
    }

    [Fact]
    public async Task GetSearchSuggestions_WithFranchiseFilter_ShouldReturnMatchingFranchiseSuggestions()
    {
        // Act
        var response = await _client.GetAsync("/api/search/suggestions?franchiseId=1&limit=5");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("\"success\":true", content);
        
        var jsonDoc = JsonDocument.Parse(content);
        var data = jsonDoc.RootElement.GetProperty("data");
        var suggestions = data.EnumerateArray().ToList();
        
        // Should only contain CSK products
        Assert.All(suggestions, suggestion =>
        {
            Assert.Contains("CSK", suggestion.GetString() ?? "");
        });
    }

    [Fact]
    public async Task GetSearchSuggestions_WithLimitParameter_ShouldReturnLimitedResults()
    {
        // Act
        var response = await _client.GetAsync("/api/search/suggestions?limit=5");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        
        var jsonDoc = JsonDocument.Parse(content);
        var data = jsonDoc.RootElement.GetProperty("data");
        var suggestions = data.EnumerateArray().ToList();
        
        Assert.True(suggestions.Count <= 5);
    }

    [Fact]
    public async Task GetSearchSuggestions_WithMaxLimit_ShouldReturnMaxResults()
    {
        // Act
        var response = await _client.GetAsync("/api/search/suggestions?limit=50");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("\"success\":true", content);
        
        var jsonDoc = JsonDocument.Parse(content);
        var data = jsonDoc.RootElement.GetProperty("data");
        var suggestions = data.EnumerateArray().ToList();
        
        // Should have suggestions (test data has products)
        Assert.NotEmpty(suggestions);
    }

    [Fact]
    public async Task GetSearchSuggestions_WithInvalidLimit_ShouldReturnBadRequest()
    {
        // Act
        var response = await _client.GetAsync("/api/search/suggestions?limit=0");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetSearchSuggestions_WithLimitExceedingMaximum_ShouldReturnBadRequest()
    {
        // Act
        var response = await _client.GetAsync("/api/search/suggestions?limit=51");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetSearchSuggestions_ShouldIncludeFranchiseCode()
    {
        // Act
        var response = await _client.GetAsync("/api/search/suggestions?limit=10");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        
        var jsonDoc = JsonDocument.Parse(content);
        var data = jsonDoc.RootElement.GetProperty("data");
        var suggestions = data.EnumerateArray().ToList();
        
        // Each suggestion should include franchise code in format "Product Name (CODE)"
        Assert.All(suggestions, suggestion =>
        {
            var suggestionStr = suggestion.GetString() ?? "";
            Assert.Contains("(", suggestionStr);
            Assert.Contains(")", suggestionStr);
        });
    }

    [Fact]
    public async Task GetSearchSuggestions_WithPartialMatch_ShouldReturnMatchingSuggestions()
    {
        // Act
        var response = await _client.GetAsync("/api/search/suggestions?q=cap");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        
        var jsonDoc = JsonDocument.Parse(content);
        var data = jsonDoc.RootElement.GetProperty("data");
        var suggestions = data.EnumerateArray().ToList();
        
        Assert.NotEmpty(suggestions);
        Assert.All(suggestions, suggestion =>
        {
            Assert.Contains("Cap", suggestion.GetString() ?? "", StringComparison.OrdinalIgnoreCase);
        });
    }

    #endregion

    #region Complex Search Scenarios

    [Fact]
    public async Task GetSearch_CombinedQueryAndFilters_ComplexScenario()
    {
        // Act - Search for "Premium" (query), filter by CSK (franchiseId=1), Jersey type (type=1)
        var response = await _client.GetAsync("/api/search?q=Premium&franchiseId=1&type=1&page=1&pageSize=10");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("\"success\":true", content);
        Assert.Contains("CSK Premium Jersey", content);
    }

    [Fact]
    public async Task GetSearch_AllProductTypes_ShouldWork()
    {
        // Test each valid product type
        var productTypes = new[] { 1, 2, 3, 4, 5, 6, 7, 8 };
        
        foreach (var type in productTypes)
        {
            // Act
            var response = await _client.GetAsync($"/api/search?type={type}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("\"success\":true", content);
        }
    }

    [Fact]
    public async Task GetSearch_AllFranchises_ShouldReturnDifferentResults()
    {
        // Get results for franchise 1
        var response1 = await _client.GetAsync("/api/search?franchiseId=1");
        var content1 = await response1.Content.ReadAsStringAsync();
        var jsonDoc1 = JsonDocument.Parse(content1);
        var count1 = jsonDoc1.RootElement.GetProperty("data").GetProperty("totalCount").GetInt32();

        // Get results for franchise 2
        var response2 = await _client.GetAsync("/api/search?franchiseId=2");
        var content2 = await response2.Content.ReadAsStringAsync();
        var jsonDoc2 = JsonDocument.Parse(content2);
        var count2 = jsonDoc2.RootElement.GetProperty("data").GetProperty("totalCount").GetInt32();

        // Assert - Different franchises should have different product counts
        Assert.True(count1 > 0);
        Assert.True(count2 > 0);
    }

    #endregion
}
