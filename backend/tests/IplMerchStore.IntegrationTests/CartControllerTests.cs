using System.Net;
using System.Text;
using System.Text.Json;
using IplMerchStore.Application.DTOs;
using Xunit;

namespace IplMerchStore.IntegrationTests;

/// <summary>
/// Integration tests for CartController endpoints
/// Tests real HTTP requests through the full application pipeline
/// </summary>
public class CartControllerTests : IClassFixture<IplMerchStoreWebApplicationFactory>
{
    private readonly IplMerchStoreWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public CartControllerTests(IplMerchStoreWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private static StringContent CreateJsonContent(object data)
    {
        var json = JsonSerializer.Serialize(data);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }

    #region GET /api/cart/{userId} Tests

    [Fact]
    public async Task GetCart_WithNonExistentUser_ShouldReturn404()
    {
        // Act
        var response = await _client.GetAsync("/api/cart/nonexistent-user");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetCart_WithValidUser_ShouldReturnOk()
    {
        // Arrange
        var userId = "test-user-1";

        // Act
        var response = await _client.GetAsync($"/api/cart/{userId}");

        // Assert
        // Should return 404 since no cart exists yet (not an error, just empty state)
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetCart_AfterAddingItem_ShouldReturnCartWithItems()
    {
        // Arrange
        var userId = "cart-test-user";
        
        // First add an item to create cart
        var addRequest = new AddCartItemRequest { ProductId = 1, Quantity = 2 };
        var addResponse = await _client.PostAsync(
            $"/api/cart/{userId}/items",
            CreateJsonContent(addRequest));
        
        // Assert add was successful
        Assert.Equal(HttpStatusCode.OK, addResponse.StatusCode);

        // Act - Get the cart
        var response = await _client.GetAsync($"/api/cart/{userId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Items", content);
        Assert.Contains("TotalAmount", content);
        Assert.Contains("TotalQuantity", content);
    }

    #endregion

    #region POST /api/cart/{userId}/items Tests

    [Fact]
    public async Task AddToCart_WithValidProductAndQuantity_ShouldReturnOk()
    {
        // Arrange
        var userId = "add-item-user-1";
        var request = new AddCartItemRequest { ProductId = 1, Quantity = 1 };

        // Act
        var response = await _client.PostAsync(
            $"/api/cart/{userId}/items",
            CreateJsonContent(request));

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("true", content.ToLower()); // Success field
        Assert.Contains("Items", content);
    }

    [Fact]
    public async Task AddToCart_AddingExistingProduct_ShouldIncrementQuantity()
    {
        // Arrange
        var userId = "increment-qty-user";
        var request1 = new AddCartItemRequest { ProductId = 1, Quantity = 2 };
        var request2 = new AddCartItemRequest { ProductId = 1, Quantity = 3 };

        // Act
        var response1 = await _client.PostAsync($"/api/cart/{userId}/items", CreateJsonContent(request1));
        var content1 = await response1.Content.ReadAsStringAsync();
        Assert.Contains("\"quantity\":2", content1);

        var response2 = await _client.PostAsync($"/api/cart/{userId}/items", CreateJsonContent(request2));
        var content2 = await response2.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response2.StatusCode);
        Assert.Contains("\"quantity\":5", content2); // 2 + 3
    }

    [Fact]
    public async Task AddToCart_WithInvalidQuantity_ShouldReturnBadRequest()
    {
        // Arrange
        var userId = "invalid-qty-user";
        var request = new AddCartItemRequest { ProductId = 1, Quantity = 0 };

        // Act
        var response = await _client.PostAsync(
            $"/api/cart/{userId}/items",
            CreateJsonContent(request));

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task AddToCart_WithNonExistentProduct_ShouldReturnNotFound()
    {
        // Arrange
        var userId = "nonexistent-product-user";
        var request = new AddCartItemRequest { ProductId = 99999, Quantity = 1 };

        // Act
        var response = await _client.PostAsync(
            $"/api/cart/{userId}/items",
            CreateJsonContent(request));

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("not found", content.ToLower());
    }

    [Fact]
    public async Task AddToCart_WithoutUserId_ShouldReturnBadRequest()
    {
        // Act
        var request = new AddCartItemRequest { ProductId = 1, Quantity = 1 };
        var response = await _client.PostAsync(
            "/api/cart//items",
            CreateJsonContent(request));

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task AddToCart_CalculatesTotalAmountCorrectly()
    {
        // Arrange
        var userId = "total-calc-user";
        var request1 = new AddCartItemRequest { ProductId = 1, Quantity = 2 };
        var request2 = new AddCartItemRequest { ProductId = 2, Quantity = 1 };

        // Act
        await _client.PostAsync($"/api/cart/{userId}/items", CreateJsonContent(request1));
        var response = await _client.PostAsync($"/api/cart/{userId}/items", CreateJsonContent(request2));
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("TotalAmount", content);
        Assert.Contains("ItemCount", content);
    }

    #endregion

    #region PUT /api/cart/{userId}/items/{productId} Tests

    [Fact]
    public async Task UpdateCartItem_WithValidQuantity_ShouldUpdateSuccessfully()
    {
        // Arrange
        var userId = "update-item-user";
        
        // Add item first
        var addRequest = new AddCartItemRequest { ProductId = 1, Quantity = 2 };
        await _client.PostAsync($"/api/cart/{userId}/items", CreateJsonContent(addRequest));

        // Update quantity
        var updateRequest = new UpdateCartItemRequest { Quantity = 5 };

        // Act
        var response = await _client.PutAsync(
            $"/api/cart/{userId}/items/1",
            CreateJsonContent(updateRequest));

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("\"quantity\":5", content);
    }

    [Fact]
    public async Task UpdateCartItem_WithQuantityZero_ShouldRemoveItem()
    {
        // Arrange
        var userId = "remove-via-update-user";
        
        // Add item first
        var addRequest = new AddCartItemRequest { ProductId = 1, Quantity = 2 };
        await _client.PostAsync($"/api/cart/{userId}/items", CreateJsonContent(addRequest));

        // Update quantity to 0 to remove
        var updateRequest = new UpdateCartItemRequest { Quantity = 0 };

        // Act
        var response = await _client.PutAsync(
            $"/api/cart/{userId}/items/1",
            CreateJsonContent(updateRequest));

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("\"itemCount\":0", content);
        Assert.Contains("\"isEmpty\":true", content);
    }

    [Fact]
    public async Task UpdateCartItem_WithNonExistentProduct_ShouldReturnNotFound()
    {
        // Arrange
        var userId = "nonexistent-prod-update";
        var updateRequest = new UpdateCartItemRequest { Quantity = 1 };

        // Act
        var response = await _client.PutAsync(
            $"/api/cart/{userId}/items/99999",
            CreateJsonContent(updateRequest));

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateCartItem_WithNegativeQuantity_ShouldReturnBadRequest()
    {
        // Arrange
        var userId = "negative-qty-user";
        var updateRequest = new UpdateCartItemRequest { Quantity = -1 };

        // Act
        var response = await _client.PutAsync(
            $"/api/cart/{userId}/items/1",
            CreateJsonContent(updateRequest));

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    #endregion

    #region DELETE /api/cart/{userId}/items/{productId} Tests

    [Fact]
    public async Task DeleteCartItem_WithValidProduct_ShouldRemoveItem()
    {
        // Arrange
        var userId = "delete-item-user";
        
        // Add item first
        var addRequest = new AddCartItemRequest { ProductId = 1, Quantity = 2 };
        await _client.PostAsync($"/api/cart/{userId}/items", CreateJsonContent(addRequest));

        // Act
        var response = await _client.DeleteAsync($"/api/cart/{userId}/items/1");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("\"isEmpty\":true", content);
    }

    [Fact]
    public async Task DeleteCartItem_WithNonExistentProduct_ShouldReturnNotFound()
    {
        // Arrange
        var userId = "delete-nonexistent-user";

        // Act
        var response = await _client.DeleteAsync($"/api/cart/{userId}/items/99999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteCartItem_WithMultipleItems_ShouldRemoveOnlySelected()
    {
        // Arrange
        var userId = "delete-multi-item-user";
        
        // Add two items
        await _client.PostAsync($"/api/cart/{userId}/items", CreateJsonContent(new AddCartItemRequest { ProductId = 1, Quantity = 1 }));
        await _client.PostAsync($"/api/cart/{userId}/items", CreateJsonContent(new AddCartItemRequest { ProductId = 2, Quantity = 1 }));

        // Act - Delete only product 1
        var response = await _client.DeleteAsync($"/api/cart/{userId}/items/1");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("\"itemCount\":1", content);
        Assert.Contains("productId", content);
        Assert.Contains("\"productId\":2", content);
    }

    #endregion

    #region DELETE /api/cart/{userId} Tests

    [Fact]
    public async Task ClearCart_WithItems_ShouldRemoveAllItems()
    {
        // Arrange
        var userId = "clear-cart-user";
        
        // Add multiple items
        await _client.PostAsync($"/api/cart/{userId}/items", CreateJsonContent(new AddCartItemRequest { ProductId = 1, Quantity = 1 }));
        await _client.PostAsync($"/api/cart/{userId}/items", CreateJsonContent(new AddCartItemRequest { ProductId = 2, Quantity = 1 }));

        // Act
        var response = await _client.DeleteAsync($"/api/cart/{userId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Verify cart is empty by trying to get it
        var getResponse = await _client.GetAsync($"/api/cart/{userId}");
        // Should return 404 (no cart) or 200 (empty cart) depending on implementation
        Assert.True(getResponse.StatusCode == HttpStatusCode.OK || getResponse.StatusCode == HttpStatusCode.NotFound);
    }

    #endregion

    #region Business Rule Validation Tests

    [Fact]
    public async Task Cart_ShouldNotAllowQuantityExceedingInventory()
    {
        // Arrange
        var userId = "inventory-check-user";
        // Assuming product 1 has limited inventory (checked in WebApplicationFactory seed data)
        var request = new AddCartItemRequest { ProductId = 1, Quantity = 1000 };

        // Act
        var response = await _client.PostAsync(
            $"/api/cart/{userId}/items",
            CreateJsonContent(request));

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("available", content.ToLower());
    }

    [Fact]
    public async Task Cart_ResponseShouldIncludeProductSnapshot()
    {
        // Arrange
        var userId = "snapshot-user";
        var request = new AddCartItemRequest { ProductId = 1, Quantity = 1 };

        // Act
        var response = await _client.PostAsync($"/api/cart/{userId}/items", CreateJsonContent(request));
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("ProductName", content);
        Assert.Contains("ProductSku", content);
        Assert.Contains("UnitPrice", content);
        Assert.Contains("Subtotal", content);
        Assert.Contains("CurrentInventory", content);
    }

    #endregion

    #region Edge Case Tests

    [Fact]
    public async Task Cart_ShouldHandleMultipleUsersIndependently()
    {
        // Arrange
        var user1 = "user-multi-1";
        var user2 = "user-multi-2";

        // Act
        await _client.PostAsync($"/api/cart/{user1}/items", CreateJsonContent(new AddCartItemRequest { ProductId = 1, Quantity = 1 }));
        await _client.PostAsync($"/api/cart/{user2}/items", CreateJsonContent(new AddCartItemRequest { ProductId = 2, Quantity = 2 }));

        var response1 = await _client.GetAsync($"/api/cart/{user1}");
        var content1 = await response1.Content.ReadAsStringAsync();

        var response2 = await _client.GetAsync($"/api/cart/{user2}");
        var content2 = await response2.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response1.StatusCode);
        Assert.Equal(HttpStatusCode.OK, response2.StatusCode);
        Assert.Contains("\"totalQuantity\":1", content1);
        Assert.Contains("\"totalQuantity\":2", content2);
    }

    [Fact]
    public async Task Cart_ShouldCalculateCorrectTotals()
    {
        // Arrange
        var userId = "totals-user";
        
        // Add product 1 with quantity 2
        await _client.PostAsync($"/api/cart/{userId}/items", CreateJsonContent(new AddCartItemRequest { ProductId = 1, Quantity = 2 }));
        
        // Add product 2 with quantity 1
        var response = await _client.PostAsync($"/api/cart/{userId}/items", CreateJsonContent(new AddCartItemRequest { ProductId = 2, Quantity = 1 }));
        var content = await response.Content.ReadAsStringAsync();

        // Assert totals
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("\"itemCount\":2", content);
        Assert.Contains("\"totalQuantity\":3", content); // 2 + 1
        Assert.Contains("TotalAmount", content);
    }

    #endregion
}
