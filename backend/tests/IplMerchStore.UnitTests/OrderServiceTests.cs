using IplMerchStore.Application.Common;
using IplMerchStore.Application.DTOs;
using IplMerchStore.Application.Interfaces;
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
/// Unit tests for OrderService
/// Tests checkout flow, order retrieval, inventory management, and cart clearing
/// </summary>
public class OrderServiceTests
{
    private readonly Mock<ICartService> _mockCartService;
    private readonly Mock<IPaymentService> _mockPaymentService;
    private readonly Mock<ILogger<OrderService>> _mockLogger;
    private readonly AppDbContext _dbContext;
    private readonly OrderService _orderService;

    public OrderServiceTests()
    {
        // Create in-memory database for testing
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _dbContext = new AppDbContext(options);
        _mockCartService = new Mock<ICartService>();
        _mockPaymentService = new Mock<IPaymentService>();
        _mockLogger = new Mock<ILogger<OrderService>>();

        _orderService = new OrderService(_dbContext, _mockCartService.Object, _mockPaymentService.Object, _mockLogger.Object);

        // Seed test data
        SeedTestData();
    }

    private void SeedTestData()
    {
        // Create test franchise
        var franchise = new Franchise
        {
            Id = 1,
            Name = "Test Franchise",
            ShortCode = "TF",
            PrimaryColor = "#FF0000",
            SecondaryColor = "#00FF00"
        };
        _dbContext.Franchises.Add(franchise);

        // Create test products
        var product1 = new Product
        {
            Id = 1,
            Name = "Test Product 1",
            Description = "Test Description 1",
            Price = 100.00m,
            Currency = "INR",
            InventoryCount = 50,
            ProductType = ProductType.Jersey,
            FranchiseId = 1,
            SKU = "TEST-001",
            IsActive = true
        };

        var product2 = new Product
        {
            Id = 2,
            Name = "Test Product 2",
            Description = "Test Description 2",
            Price = 200.00m,
            Currency = "INR",
            InventoryCount = 30,
            ProductType = ProductType.Cap,
            FranchiseId = 1,
            SKU = "TEST-002",
            IsActive = true
        };

        _dbContext.Products.AddRange(product1, product2);

        // Create test cart for user
        var cart = new Cart
        {
            Id = 1,
            UserId = "test-user",
            TotalPrice = 0
        };
        _dbContext.Carts.Add(cart);

        // Create cart items
        var cartItem1 = new CartItem
        {
            Id = 1,
            CartId = 1,
            ProductId = 1,
            Quantity = 2,
            UnitPrice = 100.00m
        };

        var cartItem2 = new CartItem
        {
            Id = 2,
            CartId = 1,
            ProductId = 2,
            Quantity = 1,
            UnitPrice = 200.00m
        };

        _dbContext.CartItems.AddRange(cartItem1, cartItem2);

        _dbContext.SaveChanges();
    }

    #region Checkout Tests

    [Fact]
    public async Task CreateOrderAsync_WithValidCart_ShouldCreateOrder()
    {
        // Arrange
        var userId = "test-user";
        var checkoutRequest = new CreateOrderDto
        {
            ShippingAddress = "123 Test Street",
            CustomerEmail = "test@example.com",
            CustomerPhone = "1234567890"
        };

        _mockPaymentService
            .Setup(x => x.ProcessPaymentAsync(It.IsAny<int>(), It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.SuccessResult(true, "Payment successful"));

        _mockCartService
            .Setup(x => x.ClearCartAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Result { Success = true, Message = "Cleared" });

        // Act
        var result = await _orderService.CreateOrderAsync(userId, checkoutRequest);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(userId, result.Data.UserId);
        Assert.Equal(400.00m, result.Data.TotalAmount); // 2*100 + 1*200
        Assert.Equal((int)OrderStatus.Pending, result.Data.Status);
        Assert.Equal(2, result.Data.Items.Count());
        Assert.Equal("123 Test Street", result.Data.ShippingAddress);
    }

    [Fact]
    public async Task CreateOrderAsync_WithEmptyCart_ShouldFail()
    {
        // Arrange
        var userId = "user-without-cart";
        var checkoutRequest = new CreateOrderDto
        {
            ShippingAddress = "123 Test Street"
        };

        // Act
        var result = await _orderService.CreateOrderAsync(userId, checkoutRequest);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Cart is empty", result.Message);
    }

    [Fact]
    public async Task CreateOrderAsync_WithInsufficientInventory_ShouldFail()
    {
        // Arrange
        // Create a cart with item that exceeds inventory
        var userId = "inventory-test-user";
        var cart = new Cart { Id = 0, UserId = userId, TotalPrice = 0 };
        _dbContext.Carts.Add(cart);
        await _dbContext.SaveChangesAsync();

        var cartItem = new CartItem
        {
            Id = 0,
            CartId = cart.Id,
            ProductId = 1,
            Quantity = 100, // Exceeds available inventory
            UnitPrice = 100.00m
        };
        _dbContext.CartItems.Add(cartItem);
        await _dbContext.SaveChangesAsync();

        var checkoutRequest = new CreateOrderDto();

        // Act
        var result = await _orderService.CreateOrderAsync(userId, checkoutRequest);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("insufficient inventory", result.Message);
    }

    [Fact]
    public async Task CreateOrderAsync_ShouldReduceInventory()
    {
        // Arrange
        var userId = "test-user";
        var product1InventoryBefore = _dbContext.Products.First(p => p.Id == 1).InventoryCount;
        var checkoutRequest = new CreateOrderDto();

        _mockPaymentService
            .Setup(x => x.ProcessPaymentAsync(It.IsAny<int>(), It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.SuccessResult(true, "Payment successful"));

        _mockCartService
            .Setup(x => x.ClearCartAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Result { Success = true, Message = "Cleared" });

        // Act
        await _orderService.CreateOrderAsync(userId, checkoutRequest);

        // Assert
        var product1InventoryAfter = _dbContext.Products.First(p => p.Id == 1).InventoryCount;
        var product2InventoryAfter = _dbContext.Products.First(p => p.Id == 2).InventoryCount;

        Assert.Equal(product1InventoryBefore - 2, product1InventoryAfter); // 50 - 2 = 48
        Assert.Equal(30 - 1, product2InventoryAfter); // 30 - 1 = 29
    }

    [Fact]
    public async Task CreateOrderAsync_ShouldClearCart()
    {
        // Arrange
        var userId = "test-user";
        var checkoutRequest = new CreateOrderDto();

        _mockPaymentService
            .Setup(x => x.ProcessPaymentAsync(It.IsAny<int>(), It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.SuccessResult(true, "Payment successful"));

        var clearCartCalled = false;
        _mockCartService
            .Setup(x => x.ClearCartAsync(userId, It.IsAny<CancellationToken>()))
            .Callback(() => clearCartCalled = true)
            .ReturnsAsync(new Result { Success = true, Message = "Cleared" });

        // Act
        await _orderService.CreateOrderAsync(userId, checkoutRequest);

        // Assert
        Assert.True(clearCartCalled);
        _mockCartService.Verify(x => x.ClearCartAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateOrderAsync_WithZeroUserId_ShouldFail()
    {
        // Arrange
        var checkoutRequest = new CreateOrderDto();

        // Act
        var result = await _orderService.CreateOrderAsync("", checkoutRequest);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("User ID is required", result.Message);
    }

    [Fact]
    public async Task CreateOrderAsync_WithPaymentSuccess_ShouldCompleteCheckout()
    {
        // Arrange
        var userId = "payment-success-user";
        var cart = new Cart { Id = 0, UserId = userId, TotalPrice = 0 };
        _dbContext.Carts.Add(cart);
        await _dbContext.SaveChangesAsync();

        var cartItem = new CartItem
        {
            Id = 0,
            CartId = cart.Id,
            ProductId = 1,
            Quantity = 1,
            UnitPrice = 100.00m
        };
        _dbContext.CartItems.Add(cartItem);
        await _dbContext.SaveChangesAsync();

        var checkoutRequest = new CreateOrderDto();

        _mockPaymentService
            .Setup(x => x.ProcessPaymentAsync(It.IsAny<int>(), It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.SuccessResult(true, "Payment successful"));

        _mockCartService
            .Setup(x => x.ClearCartAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Result { Success = true, Message = "Cleared" });

        // Act
        var result = await _orderService.CreateOrderAsync(userId, checkoutRequest);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        
        // Verify payment was called
        _mockPaymentService.Verify(
            x => x.ProcessPaymentAsync(It.IsAny<int>(), 100.00m, "INR", It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateOrderAsync_WithPaymentFailure_ShouldRollback()
    {
        // Arrange
        var userId = "payment-failure-user";
        var cart = new Cart { Id = 0, UserId = userId, TotalPrice = 0 };
        _dbContext.Carts.Add(cart);
        await _dbContext.SaveChangesAsync();

        var cartItem = new CartItem
        {
            Id = 0,
            CartId = cart.Id,
            ProductId = 1,
            Quantity = 1,
            UnitPrice = 100.00m
        };
        _dbContext.CartItems.Add(cartItem);
        await _dbContext.SaveChangesAsync();

        var checkoutRequest = new CreateOrderDto();

        _mockPaymentService
            .Setup(x => x.ProcessPaymentAsync(It.IsAny<int>(), It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.FailureResult("Payment service error"));

        // Act
        var result = await _orderService.CreateOrderAsync(userId, checkoutRequest);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Payment processing failed", result.Message);
        
        // Verify cart was NOT cleared
        _mockCartService.Verify(x => x.ClearCartAsync(userId, It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateOrderAsync_WithPaymentDeclined_ShouldReturnDeclinedError()
    {
        // Arrange
        var userId = "payment-declined-user";
        var cart = new Cart { Id = 0, UserId = userId, TotalPrice = 0 };
        _dbContext.Carts.Add(cart);
        await _dbContext.SaveChangesAsync();

        var cartItem = new CartItem
        {
            Id = 0,
            CartId = cart.Id,
            ProductId = 1,
            Quantity = 1,
            UnitPrice = 100.00m
        };
        _dbContext.CartItems.Add(cartItem);
        await _dbContext.SaveChangesAsync();

        var checkoutRequest = new CreateOrderDto();

        _mockPaymentService
            .Setup(x => x.ProcessPaymentAsync(It.IsAny<int>(), It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.SuccessResult(false, "Payment declined"));

        // Act
        var result = await _orderService.CreateOrderAsync(userId, checkoutRequest);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Payment was declined", result.Message);
        
        // Verify cart was NOT cleared
        _mockCartService.Verify(x => x.ClearCartAsync(userId, It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion

    #region Order Retrieval Tests

    [Fact]
    public async Task GetOrderByIdAsync_WithValidId_ShouldReturnOrder()
    {
        // Arrange
        var userId = "test-user";
        var checkoutRequest = new CreateOrderDto();

        _mockPaymentService
            .Setup(x => x.ProcessPaymentAsync(
                It.IsAny<int>(),
                It.IsAny<decimal>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.SuccessResult(true, "Payment successful"));

        _mockCartService
            .Setup(x => x.ClearCartAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Result { Success = true, Message = "Cleared" });

        // Create an order
        var createResult = await _orderService.CreateOrderAsync(userId, checkoutRequest);
        Assert.True(createResult.Success, $"Order creation failed: {createResult.Message}");
        var orderId = createResult.Data.Id;

        // Act
        var result = await _orderService.GetOrderByIdAsync(orderId);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(orderId, result.Data.Id);
        Assert.Equal(userId, result.Data.UserId);
    }

    [Fact]
    public async Task GetOrderByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Act
        var result = await _orderService.GetOrderByIdAsync(999);

        // Assert
        Assert.True(result.Success);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task GetUserOrdersAsync_ShouldReturnOrdersInDescendingOrder()
    {
        // Arrange
        var userId = "test-user";
        _mockCartService
            .Setup(x => x.ClearCartAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Result { Success = true, Message = "Cleared" });

        // Create multiple orders (simulate with db manipulation)
        var order1 = new Order
        {
            Id = 1,
            UserId = userId,
            Status = OrderStatus.Pending,
            TotalAmount = 100,
            CreatedAtUtc = DateTime.UtcNow.AddMinutes(-10)
        };

        var order2 = new Order
        {
            Id = 2,
            UserId = userId,
            Status = OrderStatus.Pending,
            TotalAmount = 200,
            CreatedAtUtc = DateTime.UtcNow
        };

        _dbContext.Orders.AddRange(order1, order2);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _orderService.GetUserOrdersAsync(userId, 1, 10);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.True(result.Data.Items.Count() >= 2);
        // Most recent order should be first
        Assert.True(result.Data.Items.First().CreatedAtUtc >= result.Data.Items.Last().CreatedAtUtc);
    }

    [Fact]
    public async Task GetUserOrdersAsync_WithInvalidUserId_ShouldFail()
    {
        // Act
        var result = await _orderService.GetUserOrdersAsync("", 1, 10);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("User ID is required", result.Message);
    }

    #endregion
}
