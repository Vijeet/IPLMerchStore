using System.Net;
using System.Text;
using System.Text.Json;
using IplMerchStore.Application.DTOs;
using Xunit;

namespace IplMerchStore.IntegrationTests;

/// <summary>
/// Integration tests for OrdersController endpoints
/// Tests full checkout flow, order history, and order details endpoints
/// </summary>
public class OrdersControllerTests : IClassFixture<IplMerchStoreWebApplicationFactory>
{
    private readonly IplMerchStoreWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public OrdersControllerTests(IplMerchStoreWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private static StringContent CreateJsonContent(object data)
    {
        var json = JsonSerializer.Serialize(data);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }

    #region POST /api/orders/checkout Tests

    [Fact]
    public async Task Checkout_WithValidCart_ShouldCreateOrder()
    {
        // Arrange
        var userId = "checkout-test-user-1";
        
        // First, add items to cart
        var addRequest = new AddCartItemRequest { ProductId = 1, Quantity = 1 };
        var addResponse = await _client.PostAsync(
            $"/api/cart/{userId}/items",
            CreateJsonContent(addRequest));
        Assert.Equal(HttpStatusCode.OK, addResponse.StatusCode);

        // Now checkout
        var checkoutRequest = new CheckoutRequestDto
        {
            ShippingAddress = "123 Test Street",
            CustomerEmail = "test@example.com",
            CustomerPhone = "1234567890"
        };

        // Act
        var response = await _client.PostAsync(
            $"/api/orders/checkout?userId={userId}",
            CreateJsonContent(checkoutRequest));

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("\"id\":", content);
        Assert.Contains("\"userId\":", content);
        Assert.Contains("\"totalAmount\":", content);
        Assert.Contains("\"status\":", content);
        Assert.Contains("\"items\":", content);
    }

    [Fact]
    public async Task Checkout_WithEmptyCart_ShouldReturnBadRequest()
    {
        // Arrange
        var userId = "checkout-empty-user";
        
        var checkoutRequest = new CheckoutRequestDto
        {
            ShippingAddress = "123 Test Street"
        };

        // Act
        var response = await _client.PostAsync(
            $"/api/orders/checkout?userId={userId}",
            CreateJsonContent(checkoutRequest));

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Cart is empty", content);
    }

    [Fact]
    public async Task Checkout_WithoutUserId_ShouldReturnBadRequest()
    {
        // Arrange
        var checkoutRequest = new CheckoutRequestDto();

        // Act
        var response = await _client.PostAsync(
            "/api/orders/checkout",
            CreateJsonContent(checkoutRequest));

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("User ID is required", content);
    }

    [Fact]
    public async Task Checkout_ShouldReduceInventory()
    {
        // Arrange  
        var userId = "checkout-inventory-user";
        
        // Get product details before checkout
        var getProductResponse = await _client.GetAsync("/api/products/1");
        var productContentBefore = await getProductResponse.Content.ReadAsStringAsync();
        var productBefore = JsonSerializer.Deserialize<ProductDetailDto>(productContentBefore);
        var inventoryBefore = productBefore?.InventoryCount ?? 0;

        // Add to cart and checkout
        var addRequest = new AddCartItemRequest { ProductId = 1, Quantity = 2 };
        await _client.PostAsync(
            $"/api/cart/{userId}/items",
            CreateJsonContent(addRequest));

        var checkoutRequest = new CheckoutRequestDto();
        await _client.PostAsync(
            $"/api/orders/checkout?userId={userId}",
            CreateJsonContent(checkoutRequest));

        // Get product details after checkout
        var getProductResponseAfter = await _client.GetAsync("/api/products/1");
        var productContentAfter = await getProductResponseAfter.Content.ReadAsStringAsync();
        var productAfter = JsonSerializer.Deserialize<ProductDetailDto>(productContentAfter);
        var inventoryAfter = productAfter?.InventoryCount ?? 0;

        // Assert
        Assert.Equal(inventoryBefore - 2, inventoryAfter);
    }

    [Fact]
    public async Task Checkout_ShouldClearCart()
    {
        // Arrange
        var userId = "checkout-clear-cart-user";
        
        // Add items to cart
        var addRequest = new AddCartItemRequest { ProductId = 1, Quantity = 1 };
        await _client.PostAsync(
            $"/api/cart/{userId}/items",
            CreateJsonContent(addRequest));

        // Verify cart has items
        var cartBefore = await _client.GetAsync($"/api/cart/{userId}");
        Assert.Equal(HttpStatusCode.OK, cartBefore.StatusCode);

        // Checkout
        var checkoutRequest = new CheckoutRequestDto();
        var checkoutResponse = await _client.PostAsync(
            $"/api/orders/checkout?userId={userId}",
            CreateJsonContent(checkoutRequest));
        Assert.Equal(HttpStatusCode.OK, checkoutResponse.StatusCode);

        // Act - Get cart after checkout
        var cartAfter = await _client.GetAsync($"/api/cart/{userId}");

        // Assert - cart should return 404 (no cart after clearing)
        Assert.Equal(HttpStatusCode.NotFound, cartAfter.StatusCode);
    }

    #endregion

    #region GET /api/orders/{userId} Tests

    [Fact]
    public async Task GetUserOrders_WithValidUser_ShouldReturnOrders()
    {
        // Arrange
        var userId = "order-history-test-user";
        
        // Create an order
        var addRequest = new AddCartItemRequest { ProductId = 1, Quantity = 1 };
        await _client.PostAsync(
            $"/api/cart/{userId}/items",
            CreateJsonContent(addRequest));

        var checkoutRequest = new CheckoutRequestDto();
        var checkoutResponse = await _client.PostAsync(
            $"/api/orders/checkout?userId={userId}",
            CreateJsonContent(checkoutRequest));
        Assert.Equal(HttpStatusCode.OK, checkoutResponse.StatusCode);

        // Act
        var response = await _client.GetAsync($"/api/orders/{userId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("\"items\":", content);
        Assert.Contains("\"pageNumber\":", content);
        Assert.Contains("\"pageSize\":", content);
        Assert.Contains("\"totalCount\":", content);
    }

    [Fact]
    public async Task GetUserOrders_WithNonExistentUser_ShouldReturnEmptyList()
    {
        // Arrange
        var userId = "non-existent-user-" + Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/orders/{userId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("\"totalCount\":0", content);
    }

    [Fact]
    public async Task GetUserOrders_WithoutUserId_ShouldReturnBadRequest()
    {
        // Act
        var response = await _client.GetAsync("/api/orders/");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetUserOrders_WithPagination_ShouldReturnPagedResults()
    {
        // Arrange
        var userId = "pagination-test-user";

        // Act
        var response = await _client.GetAsync($"/api/orders/{userId}?pageNumber=1&pageSize=5");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("\"pageNumber\":1", content);
        Assert.Contains("\"pageSize\":5", content);
    }

    #endregion

    #region GET /api/orders/{userId}/{orderId} Tests

    [Fact]
    public async Task GetOrderDetail_WithValidOrderId_ShouldReturnOrderDetails()
    {
        // Arrange
        var userId = "order-detail-test-user";
        
        // Create an order
        var addRequest = new AddCartItemRequest { ProductId = 1, Quantity = 2 };
        await _client.PostAsync(
            $"/api/cart/{userId}/items",
            CreateJsonContent(addRequest));

        var checkoutRequest = new CheckoutRequestDto
        {
            ShippingAddress = "456 Detail Street",
            CustomerEmail = "detail@example.com"
        };

        var checkoutResponse = await _client.PostAsync(
            $"/api/orders/checkout?userId={userId}",
            CreateJsonContent(checkoutRequest));
        
        var checkoutContent = await checkoutResponse.Content.ReadAsStringAsync();
        var orderDto = JsonSerializer.Deserialize<OrderDto>(checkoutContent);
        var orderId = orderDto?.Id;

        // Act
        var response = await _client.GetAsync($"/api/orders/{userId}/{orderId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("\"id\":", content);
        Assert.Contains("\"userId\":", content);
        Assert.Contains("\"items\":", content);
        Assert.Contains("456 Detail Street", content);
        Assert.Contains("detail@example.com", content);
    }

    [Fact]
    public async Task GetOrderDetail_WithInvalidOrderId_ShouldReturnNotFound()
    {
        // Arrange
        var userId = "order-detail-invalid-user";

        // Act
        var response = await _client.GetAsync($"/api/orders/{userId}/99999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetOrderDetail_WithDifferentUserId_ShouldReturnNotFound()
    {
        // Arrange
        var userId1 = "order-detail-user-1";
        var userId2 = "order-detail-user-2";
        
        // Create order for userId1
        var addRequest = new AddCartItemRequest { ProductId = 1, Quantity = 1 };
        await _client.PostAsync(
            $"/api/cart/{userId1}/items",
            CreateJsonContent(addRequest));

        var checkoutRequest = new CheckoutRequestDto();
        var checkoutResponse = await _client.PostAsync(
            $"/api/orders/checkout?userId={userId1}",
            CreateJsonContent(checkoutRequest));

        var checkoutContent = await checkoutResponse.Content.ReadAsStringAsync();
        var orderDto = JsonSerializer.Deserialize<OrderDto>(checkoutContent);
        var orderId = orderDto?.Id;

        // Act - Try to access order from userId2
        var response = await _client.GetAsync($"/api/orders/{userId2}/{orderId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion
}
