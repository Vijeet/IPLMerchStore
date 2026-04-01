using System.Net;

namespace IplMerchStore.IntegrationTests;

/// <summary>
/// Smoke test to verify the API starts correctly
/// </summary>
public class SmokeTests : IClassFixture<IplMerchStoreWebApplicationFactory>
{
    private readonly IplMerchStoreWebApplicationFactory _factory;

    public SmokeTests(IplMerchStoreWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task ApiShouldStart_AndHealthEndpointRespond()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/health");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.NotEmpty(content);
        Assert.Contains("Healthy", content);
    }

    [Fact]
    public async Task SwaggerUiShouldBeAccessible()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/");

        // Assert
        Assert.True(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Redirect);
    }

    [Fact]
    public async Task NonExistentEndpointShouldReturn404()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/nonexistent");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
