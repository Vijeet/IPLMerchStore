using IplMerchStore.Application.Common;
using IplMerchStore.Application.DTOs;
using IplMerchStore.Application.Interfaces;
using IplMerchStore.Domain.Entities;
using IplMerchStore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IplMerchStore.Infrastructure.Services;

/// <summary>
/// Service for managing user shopping carts
/// 
/// Implementation Notes:
/// - Cart is created on first item addition (lazy creation)
/// - One active cart per user (userId is the key)
/// - Items are stored with UnitPrice snapshot for historical pricing
/// - Quantities are validated against current product inventory
/// - All cart operations are transactional at the service level
/// 
/// Business Rules Enforced:
/// 1. Product must exist and be active (IsActive = true)
/// 2. Product must have available inventory (InventoryCount > 0 for new items)
/// 3. Requested quantity must be > 0 for additions
/// 4. Total quantity (existing + new) cannot exceed inventory
/// 5. Setting quantity to 0 removes the item
/// 6. Cart is empty-state friendly for frontend (no null collections)
/// </summary>
public class CartService : ICartService
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<CartService> _logger;

    public CartService(AppDbContext dbContext, ILogger<CartService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<CartResponse?>> GetCartByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate userId
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Result<CartResponse?>.FailureResult("User ID is required");
            }

            var cart = await _dbContext.Carts
                .Where(c => c.UserId == userId)
                .Include(c => c.Items)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(cancellationToken);

            // Return null if cart doesn't exist (not an error state)
            if (cart == null)
            {
                return Result<CartResponse?>.SuccessResult(null, "No active cart found for user");
            }

            var cartResponse = MapCartToResponse(cart);
            return Result<CartResponse?>.SuccessResult(cartResponse, "Cart retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving cart for userId: {UserId}", userId);
            return Result<CartResponse?>.FailureResult("An error occurred while retrieving cart");
        }
    }

    public async Task<Result<CartResponse>> AddToCartAsync(
        string userId,
        AddCartItemRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Result<CartResponse>.FailureResult("User ID is required");
            }

            if (request.ProductId <= 0)
            {
                return Result<CartResponse>.FailureResult("Product ID must be greater than 0");
            }

            if (request.Quantity <= 0)
            {
                return Result<CartResponse>.FailureResult("Quantity must be greater than 0");
            }

            // Fetch the product
            var product = await _dbContext.Products
                .FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken);

            if (product == null)
            {
                return Result<CartResponse>.FailureResult($"Product with ID {request.ProductId} not found");
            }

            // Validate product is active
            if (!product.IsActive)
            {
                return Result<CartResponse>.FailureResult($"Product '{product.Name}' is no longer available");
            }

            // Get or create cart for user
            var cart = await _dbContext.Carts
                .Where(c => c.UserId == userId)
                .Include(c => c.Items)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(cancellationToken);

            if (cart == null)
            {
                cart = new Cart { Id = 0, UserId = userId, TotalPrice = 0 };
                _dbContext.Carts.Add(cart);
                await _dbContext.SaveChangesAsync(cancellationToken);
            }

            // Check if product already in cart
            var existingItem = cart.Items.FirstOrDefault(ci => ci.ProductId == request.ProductId);

            if (existingItem != null)
            {
                // Validate that new total quantity doesn't exceed inventory
                var newTotalQuantity = existingItem.Quantity + request.Quantity;
                if (newTotalQuantity > product.InventoryCount)
                {
                    return Result<CartResponse>.FailureResult(
                        $"Cannot add {request.Quantity} units. Only {product.InventoryCount - existingItem.Quantity} units available in stock");
                }

                // Update quantity
                existingItem.Quantity = newTotalQuantity;
                existingItem.UnitPrice = product.Price; // Update unit price to current
                _dbContext.CartItems.Update(existingItem);
            }
            else
            {
                // Validate inventory for new item
                if (request.Quantity > product.InventoryCount)
                {
                    return Result<CartResponse>.FailureResult(
                        $"Cannot add {request.Quantity} units. Only {product.InventoryCount} units available in stock");
                }

                // Add new item
                var newItem = new CartItem
                {
                    Id = 0,
                    CartId = cart.Id,
                    ProductId = product.Id,
                    Quantity = request.Quantity,
                    UnitPrice = product.Price
                };

                _dbContext.CartItems.Add(newItem);
            }

            // Update cart total and save
            await _dbContext.SaveChangesAsync(cancellationToken);

            // Reload cart with updated items for response
            cart = await _dbContext.Carts
                .Where(c => c.Id == cart.Id)
                .Include(c => c.Items)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(cancellationToken);

            var cartResponse = MapCartToResponse(cart!);
            _logger.LogInformation("Added product {ProductId} to cart for user {UserId}", request.ProductId, userId);

            return Result<CartResponse>.SuccessResult(cartResponse, "Item added to cart successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding item to cart for userId: {UserId}", userId);
            return Result<CartResponse>.FailureResult("An error occurred while adding item to cart");
        }
    }

    public async Task<Result<CartResponse>> UpdateCartItemAsync(
        string userId,
        int productId,
        UpdateCartItemRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Result<CartResponse>.FailureResult("User ID is required");
            }

            if (productId <= 0)
            {
                return Result<CartResponse>.FailureResult("Product ID must be greater than 0");
            }

            if (request.Quantity < 0)
            {
                return Result<CartResponse>.FailureResult("Quantity cannot be negative");
            }

            // Get cart
            var cart = await _dbContext.Carts
                .Where(c => c.UserId == userId)
                .Include(c => c.Items)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(cancellationToken);

            if (cart == null)
            {
                return Result<CartResponse>.FailureResult("Cart not found for user");
            }

            // Find cart item
            var cartItem = cart.Items.FirstOrDefault(ci => ci.ProductId == productId);

            if (cartItem == null)
            {
                return Result<CartResponse>.FailureResult($"Product with ID {productId} not found in cart");
            }

            // Handle removal (quantity = 0)
            if (request.Quantity == 0)
            {
                _dbContext.CartItems.Remove(cartItem);
                await _dbContext.SaveChangesAsync(cancellationToken);

                // Reload cart
                cart = await _dbContext.Carts
                    .Where(c => c.Id == cart.Id)
                    .Include(c => c.Items)
                    .ThenInclude(ci => ci.Product)
                    .FirstOrDefaultAsync(cancellationToken);

                var cartResponse = MapCartToResponse(cart!);
                _logger.LogInformation("Removed product {ProductId} from cart for user {UserId}", productId, userId);

                return Result<CartResponse>.SuccessResult(cartResponse, "Item removed from cart successfully");
            }

            // Update quantity - validate against inventory
            var product = cartItem.Product;
            if (product == null)
            {
                return Result<CartResponse>.FailureResult("Product information is missing");
            }

            if (request.Quantity > product.InventoryCount)
            {
                return Result<CartResponse>.FailureResult(
                    $"Cannot update to {request.Quantity} units. Only {product.InventoryCount} units available in stock");
            }

            cartItem.Quantity = request.Quantity;
            cartItem.UnitPrice = product.Price; // Update to current price
            _dbContext.CartItems.Update(cartItem);
            await _dbContext.SaveChangesAsync(cancellationToken);

            // Reload cart
            cart = await _dbContext.Carts
                .Where(c => c.Id == cart.Id)
                .Include(c => c.Items)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(cancellationToken);

            var updatedCartResponse = MapCartToResponse(cart!);
            _logger.LogInformation("Updated cart item {ProductId} quantity to {Quantity} for user {UserId}", productId, request.Quantity, userId);

            return Result<CartResponse>.SuccessResult(updatedCartResponse, "Cart item updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating cart item for userId: {UserId}", userId);
            return Result<CartResponse>.FailureResult("An error occurred while updating cart item");
        }
    }

    public async Task<Result<CartResponse>> RemoveFromCartAsync(
        string userId,
        int productId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Simply use UpdateCartItemAsync with quantity = 0
            return await UpdateCartItemAsync(userId, productId, new UpdateCartItemRequest { Quantity = 0 }, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing item from cart for userId: {UserId}", userId);
            return Result<CartResponse>.FailureResult("An error occurred while removing item from cart");
        }
    }

    public async Task<Result> ClearCartAsync(string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate userId
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Result.FailureResult("User ID is required");
            }

            var cart = await _dbContext.Carts
                .Where(c => c.UserId == userId)
                .Include(c => c.Items)
                .FirstOrDefaultAsync(cancellationToken);

            if (cart == null)
            {
                return Result.SuccessResult("No cart to clear");
            }

            // Remove all items from cart
            _dbContext.CartItems.RemoveRange(cart.Items);
            await _dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Cleared cart for user {UserId}", userId);
            return Result.SuccessResult("Cart cleared successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing cart for userId: {UserId}", userId);
            return Result.FailureResult("An error occurred while clearing cart");
        }
    }

    /// <summary>
    /// Maps a Cart entity to CartResponse DTO with calculated totals
    /// </summary>
    private static CartResponse MapCartToResponse(Cart cart)
    {
        var items = cart.Items
            .Where(ci => ci.Product != null)
            .Select(ci => new CartItemResponse
            {
                Id = ci.Id,
                ProductId = ci.ProductId,
                ProductName = ci.Product!.Name,
                ProductImageUrl = ci.Product.ImageUrl,
                ProductSku = ci.Product.SKU,
                UnitPrice = ci.UnitPrice,
                Quantity = ci.Quantity,
                Subtotal = ci.UnitPrice * ci.Quantity,
                CurrentInventory = ci.Product.InventoryCount,
                IsProductActive = ci.Product.IsActive
            })
            .ToList();

        var totalQuantity = items.Sum(i => i.Quantity);
        var totalAmount = items.Sum(i => i.Subtotal);

        return new CartResponse
        {
            Id = cart.Id,
            UserId = cart.UserId,
            Items = items,
            ItemCount = items.Count,
            TotalQuantity = totalQuantity,
            TotalAmount = totalAmount,
            Currency = "INR",
            IsEmpty = items.Count == 0
        };
    }
}
