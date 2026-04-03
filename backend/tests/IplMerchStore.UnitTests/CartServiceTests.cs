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
/// Unit tests for CartService business logic
/// Tests cover all documented business rules and edge cases
/// </summary>
public class CartServiceTests
{
    #region Fixtures and Setup

    private readonly DbContextOptions<AppDbContext> _dbContextOptions;

    public CartServiceTests()
    {
        // Use in-memory database for testing
        _dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    private AppDbContext CreateDbContext()
    {
        return new AppDbContext(_dbContextOptions);
    }

    private CartService CreateCartService(AppDbContext dbContext)
    {
        var mockLogger = new Mock<ILogger<CartService>>();
        return new CartService(dbContext, mockLogger.Object);
    }

    private Product CreateTestProduct(int id = 1, int inventory = 10, bool isActive = true, decimal price = 100m)
    {
        return new Product
        {
            Id = id,
            Name = $"Test Product {id}",
            Description = "Test Description",
            Price = price,
            Currency = "INR",
            InventoryCount = inventory,
            ProductType = ProductType.Jersey,
            FranchiseId = 1,
            ImageUrl = "https://example.com/image.jpg",
            IsActive = isActive,
            SKU = $"SKU{id:000}"
        };
    }

    private Franchise CreateTestFranchise(int id = 1)
    {
        return new Franchise
        {
            Id = id,
            Name = "Test Franchise",
            ShortCode = "TSF",
            PrimaryColor = "#FF0000",
            SecondaryColor = "#FFFFFF",
            CreatedAtUtc = DateTime.UtcNow
        };
    }

    #endregion

    #region GetCartByUserIdAsync Tests

    [Fact]
    public async Task GetCartByUserIdAsync_WithValidUserId_ShouldReturnCart()
    {
        // Arrange
        var dbContext = CreateDbContext();
        var service = CreateCartService(dbContext);
        var userId = "user123";
        
        var franchise = CreateTestFranchise();
        var product = CreateTestProduct();
        dbContext.Franchises.Add(franchise);
        dbContext.Products.Add(product);
        
        var cart = new Cart { Id = 0, UserId = userId, TotalPrice = 0m };
        dbContext.Carts.Add(cart);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await service.GetCartByUserIdAsync(userId);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(userId, result.Data.UserId);
    }

    [Fact]
    public async Task GetCartByUserIdAsync_WithNonExistentUser_ShouldReturnNull()
    {
        // Arrange
        var dbContext = CreateDbContext();
        var service = CreateCartService(dbContext);

        // Act
        var result = await service.GetCartByUserIdAsync("nonexistent");

        // Assert
        Assert.True(result.Success);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task GetCartByUserIdAsync_WithEmptyUserId_ShouldReturnFailure()
    {
        // Arrange
        var dbContext = CreateDbContext();
        var service = CreateCartService(dbContext);

        // Act
        var result = await service.GetCartByUserIdAsync(string.Empty);

        // Assert
        Assert.False(result.Success);
        Assert.Null(result.Data);
    }

    #endregion

    #region AddToCartAsync Tests

    [Fact]
    public async Task AddToCartAsync_WithValidProductAndQuantity_ShouldAddItem()
    {
        // Arrange
        var dbContext = CreateDbContext();
        var service = CreateCartService(dbContext);
        var userId = "user123";
        
        var franchise = CreateTestFranchise();
        var product = CreateTestProduct();
        dbContext.Franchises.Add(franchise);
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync();

        var request = new AddCartItemRequest { ProductId = product.Id, Quantity = 2 };

        // Act
        var result = await service.AddToCartAsync(userId, request);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Single(result.Data.Items);
        var item = result.Data.Items.First();
        Assert.Equal(product.Id, item.ProductId);
        Assert.Equal(2, item.Quantity);
        Assert.Equal(200m, result.Data.TotalAmount);
    }

    [Fact]
    public async Task AddToCartAsync_CreatesCartIfNotExists()
    {
        // Arrange
        var dbContext = CreateDbContext();
        var service = CreateCartService(dbContext);
        var userId = "newuser";
        
        var franchise = CreateTestFranchise();
        var product = CreateTestProduct();
        dbContext.Franchises.Add(franchise);
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync();

        var request = new AddCartItemRequest { ProductId = product.Id, Quantity = 1 };

        // Act
        var result = await service.AddToCartAsync(userId, request);

        // Assert
        Assert.True(result.Success);
        var cart = await dbContext.Carts.FirstOrDefaultAsync(c => c.UserId == userId);
        Assert.NotNull(cart);
    }

    [Fact]
    public async Task AddToCartAsync_WithExistingProduct_ShouldIncrementQuantity()
    {
        // Arrange
        var dbContext = CreateDbContext();
        var service = CreateCartService(dbContext);
        var userId = "user123";
        
        var franchise = CreateTestFranchise();
        var product = CreateTestProduct();
        dbContext.Franchises.Add(franchise);
        dbContext.Products.Add(product);
        
        var cart = new Cart { Id = 0, UserId = userId, TotalPrice = 0m };
        var cartItem = new CartItem { Id = 0, CartId = 0, ProductId = product.Id, Quantity = 2, UnitPrice = 100m };
        dbContext.Carts.Add(cart);
        await dbContext.SaveChangesAsync();
        
        cartItem.CartId = cart.Id;
        dbContext.CartItems.Add(cartItem);
        await dbContext.SaveChangesAsync();

        var request = new AddCartItemRequest { ProductId = product.Id, Quantity = 3 };

        // Act
        var result = await service.AddToCartAsync(userId, request);

        // Assert
        Assert.True(result.Success);
        Assert.Single(result.Data.Items);
        var item = result.Data.Items.First();
        Assert.Equal(5, item.Quantity); // 2 + 3
        Assert.Equal(500m, result.Data.TotalAmount); // 5 * 100
    }

    [Fact]
    public async Task AddToCartAsync_WithInactiveProduct_ShouldFail()
    {
        // Arrange
        var dbContext = CreateDbContext();
        var service = CreateCartService(dbContext);
        
        var franchise = CreateTestFranchise();
        var product = CreateTestProduct(isActive: false);
        dbContext.Franchises.Add(franchise);
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync();

        var request = new AddCartItemRequest { ProductId = product.Id, Quantity = 1 };

        // Act
        var result = await service.AddToCartAsync("user123", request);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("no longer available", result.Message);
    }

    [Fact]
    public async Task AddToCartAsync_WithNonExistentProduct_ShouldFail()
    {
        // Arrange
        var dbContext = CreateDbContext();
        var service = CreateCartService(dbContext);
        
        var request = new AddCartItemRequest { ProductId = 999, Quantity = 1 };

        // Act
        var result = await service.AddToCartAsync("user123", request);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("not found", result.Message);
    }

    [Fact]
    public async Task AddToCartAsync_WithQuantityExceedingInventory_ShouldFail()
    {
        // Arrange
        var dbContext = CreateDbContext();
        var service = CreateCartService(dbContext);
        
        var franchise = CreateTestFranchise();
        var product = CreateTestProduct(inventory: 5);
        dbContext.Franchises.Add(franchise);
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync();

        var request = new AddCartItemRequest { ProductId = product.Id, Quantity = 10 };

        // Act
        var result = await service.AddToCartAsync("user123", request);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("only", result.Message!.ToLower());
    }

    [Fact]
    public async Task AddToCartAsync_WithInvalidQuantity_ShouldFail()
    {
        // Arrange
        var dbContext = CreateDbContext();
        var service = CreateCartService(dbContext);
        
        var franchise = CreateTestFranchise();
        var product = CreateTestProduct();
        dbContext.Franchises.Add(franchise);
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync();

        var request = new AddCartItemRequest { ProductId = product.Id, Quantity = 0 };

        // Act
        var result = await service.AddToCartAsync("user123", request);

        // Assert
        Assert.False(result.Success);
    }

    [Fact]
    public async Task AddToCartAsync_CapturesUnitPrice()
    {
        // Arrange
        var dbContext = CreateDbContext();
        var service = CreateCartService(dbContext);
        
        var franchise = CreateTestFranchise();
        var product = CreateTestProduct(price: 250m);
        dbContext.Franchises.Add(franchise);
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync();

        var request = new AddCartItemRequest { ProductId = product.Id, Quantity = 1 };

        // Act
        var result = await service.AddToCartAsync("user123", request);

        // Assert
        Assert.True(result.Success);
        var item = result.Data.Items.First();
        Assert.Equal(250m, item.UnitPrice);
        Assert.Equal(250m, item.Subtotal);
    }

    #endregion

    #region UpdateCartItemAsync Tests

    [Fact]
    public async Task UpdateCartItemAsync_WithValidQuantity_ShouldUpdateItem()
    {
        // Arrange
        var dbContext = CreateDbContext();
        var service = CreateCartService(dbContext);
        var userId = "user123";
        
        var franchise = CreateTestFranchise();
        var product = CreateTestProduct();
        dbContext.Franchises.Add(franchise);
        dbContext.Products.Add(product);
        
        var cart = new Cart { Id = 0, UserId = userId, TotalPrice = 0m };
        var cartItem = new CartItem { Id = 0, CartId = 0, ProductId = product.Id, Quantity = 2, UnitPrice = 100m };
        dbContext.Carts.Add(cart);
        await dbContext.SaveChangesAsync();
        
        cartItem.CartId = cart.Id;
        dbContext.CartItems.Add(cartItem);
        await dbContext.SaveChangesAsync();

        var request = new UpdateCartItemRequest { Quantity = 5 };

        // Act
        var result = await service.UpdateCartItemAsync(userId, product.Id, request);

        // Assert
        Assert.True(result.Success);
        var item = result.Data.Items.First();
        Assert.Equal(5, item.Quantity);
        Assert.Equal(500m, result.Data.TotalAmount);
    }

    [Fact]
    public async Task UpdateCartItemAsync_WithQuantityZero_ShouldRemoveItem()
    {
        // Arrange
        var dbContext = CreateDbContext();
        var service = CreateCartService(dbContext);
        var userId = "user123";
        
        var franchise = CreateTestFranchise();
        var product = CreateTestProduct();
        dbContext.Franchises.Add(franchise);
        dbContext.Products.Add(product);
        
        var cart = new Cart { Id = 0, UserId = userId, TotalPrice = 0m };
        var cartItem = new CartItem { Id = 0, CartId = 0, ProductId = product.Id, Quantity = 2, UnitPrice = 100m };
        dbContext.Carts.Add(cart);
        await dbContext.SaveChangesAsync();
        
        cartItem.CartId = cart.Id;
        dbContext.CartItems.Add(cartItem);
        await dbContext.SaveChangesAsync();

        var request = new UpdateCartItemRequest { Quantity = 0 };

        // Act
        var result = await service.UpdateCartItemAsync(userId, product.Id, request);

        // Assert
        Assert.True(result.Success);
        Assert.Empty(result.Data.Items);
        Assert.True(result.Data.IsEmpty);
    }

    [Fact]
    public async Task UpdateCartItemAsync_WithNonExistentProduct_ShouldFail()
    {
        // Arrange
        var dbContext = CreateDbContext();
        var service = CreateCartService(dbContext);
        
        var franchise = CreateTestFranchise();
        var product = CreateTestProduct();
        dbContext.Franchises.Add(franchise);
        dbContext.Products.Add(product);
        
        var cart = new Cart { Id = 0, UserId = "user123", TotalPrice = 0m };
        dbContext.Carts.Add(cart);
        await dbContext.SaveChangesAsync();

        var request = new UpdateCartItemRequest { Quantity = 1 };

        // Act
        var result = await service.UpdateCartItemAsync("user123", 999, request);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("not found", result.Message);
    }

    [Fact]
    public async Task UpdateCartItemAsync_WithQuantityExceedingInventory_ShouldFail()
    {
        // Arrange
        var dbContext = CreateDbContext();
        var service = CreateCartService(dbContext);
        var userId = "user123";
        
        var franchise = CreateTestFranchise();
        var product = CreateTestProduct(inventory: 5);
        dbContext.Franchises.Add(franchise);
        dbContext.Products.Add(product);
        
        var cart = new Cart { Id = 0, UserId = userId, TotalPrice = 0m };
        var cartItem = new CartItem { Id = 0, CartId = 0, ProductId = product.Id, Quantity = 2, UnitPrice = 100m };
        dbContext.Carts.Add(cart);
        await dbContext.SaveChangesAsync();
        
        cartItem.CartId = cart.Id;
        dbContext.CartItems.Add(cartItem);
        await dbContext.SaveChangesAsync();

        var request = new UpdateCartItemRequest { Quantity = 10 };

        // Act
        var result = await service.UpdateCartItemAsync(userId, product.Id, request);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("only", result.Message!.ToLower());
    }

    #endregion

    #region RemoveFromCartAsync Tests

    [Fact]
    public async Task RemoveFromCartAsync_WithValidProduct_ShouldRemoveItem()
    {
        // Arrange
        var dbContext = CreateDbContext();
        var service = CreateCartService(dbContext);
        var userId = "user123";
        
        var franchise = CreateTestFranchise();
        var product = CreateTestProduct();
        dbContext.Franchises.Add(franchise);
        dbContext.Products.Add(product);
        
        var cart = new Cart { Id = 0, UserId = userId, TotalPrice = 0m };
        var cartItem = new CartItem { Id = 0, CartId = 0, ProductId = product.Id, Quantity = 2, UnitPrice = 100m };
        dbContext.Carts.Add(cart);
        await dbContext.SaveChangesAsync();
        
        cartItem.CartId = cart.Id;
        dbContext.CartItems.Add(cartItem);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await service.RemoveFromCartAsync(userId, product.Id);

        // Assert
        Assert.True(result.Success);
        Assert.Empty(result.Data.Items);
    }

    #endregion

    #region ClearCartAsync Tests

    [Fact]
    public async Task ClearCartAsync_WithValidCart_ShouldRemoveAllItems()
    {
        // Arrange
        var dbContext = CreateDbContext();
        var service = CreateCartService(dbContext);
        var userId = "user123";
        
        var franchise = CreateTestFranchise();
        var product1 = CreateTestProduct(id: 1);
        var product2 = CreateTestProduct(id: 2);
        dbContext.Franchises.Add(franchise);
        dbContext.Products.Add(product1);
        dbContext.Products.Add(product2);
        
        var cart = new Cart { Id = 0, UserId = userId, TotalPrice = 0m };
        dbContext.Carts.Add(cart);
        await dbContext.SaveChangesAsync();
        
        var item1 = new CartItem { Id = 0, CartId = cart.Id, ProductId = product1.Id, Quantity = 1, UnitPrice = 100m };
        var item2 = new CartItem { Id = 0, CartId = cart.Id, ProductId = product2.Id, Quantity = 2, UnitPrice = 100m };
        dbContext.CartItems.Add(item1);
        dbContext.CartItems.Add(item2);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await service.ClearCartAsync(userId);

        // Assert
        Assert.True(result.Success);
        var cartItems = await dbContext.CartItems.Where(ci => ci.CartId == cart.Id).ToListAsync();
        Assert.Empty(cartItems);
    }

    #endregion

    #region Cart Response Tests

    [Fact]
    public async Task CartResponse_ShouldCalculateTotalsCorrectly()
    {
        // Arrange
        var dbContext = CreateDbContext();
        var service = CreateCartService(dbContext);
        
        var franchise = CreateTestFranchise();
        var product1 = CreateTestProduct(id: 1, price: 100m);
        var product2 = CreateTestProduct(id: 2, price: 250m);
        dbContext.Franchises.Add(franchise);
        dbContext.Products.Add(product1);
        dbContext.Products.Add(product2);
        await dbContext.SaveChangesAsync();

        var request1 = new AddCartItemRequest { ProductId = product1.Id, Quantity = 2 };
        var request2 = new AddCartItemRequest { ProductId = product2.Id, Quantity = 1 };

        // Act
        var result1 = await service.AddToCartAsync("user123", request1);
        var result2 = await service.AddToCartAsync("user123", request2);

        // Assert
        Assert.Equal(3, result2.Data.TotalQuantity); // 2 + 1
        Assert.Equal(450m, result2.Data.TotalAmount); // (100 * 2) + (250 * 1)
        Assert.Equal(2, result2.Data.ItemCount);
        Assert.False(result2.Data.IsEmpty);
    }

    [Fact]
    public async Task CartResponse_ShouldIncludeProductSnapshot()
    {
        // Arrange
        var dbContext = CreateDbContext();
        var service = CreateCartService(dbContext);
        
        var franchise = CreateTestFranchise();
        var product = CreateTestProduct(price: 500m);
        dbContext.Franchises.Add(franchise);
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync();

        var request = new AddCartItemRequest { ProductId = product.Id, Quantity = 1 };

        // Act
        var result = await service.AddToCartAsync("user123", request);

        // Assert
        var item = result.Data.Items.First();
        Assert.Equal(product.Name, item.ProductName);
        Assert.Equal(product.SKU, item.ProductSku);
        Assert.Equal(product.ImageUrl, item.ProductImageUrl);
        Assert.Equal(product.InventoryCount, item.CurrentInventory);
        Assert.True(item.IsProductActive);
    }

    #endregion
}
