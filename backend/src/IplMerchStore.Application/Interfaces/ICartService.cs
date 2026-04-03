using IplMerchStore.Application.Common;
using IplMerchStore.Application.DTOs;

namespace IplMerchStore.Application.Interfaces;

/// <summary>
/// Service interface for managing user shopping carts
/// 
/// Design Notes:
/// - Each user has exactly one active cart per session
/// - Cart is created on first item addition
/// - Items are uniquely identified by ProductId within a cart
/// - Adding existing product increases quantity, captured at UnitPrice of current product
/// - Quantity cannot exceed available inventory
/// - Quantity of 0 in update operation removes the item
/// - All operations are transactional at the service level
/// </summary>
public interface ICartService
{
    /// <summary>
    /// Get the active cart for a user by user ID
    /// </summary>
    /// <param name="userId">User identifier (no authentication required)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Cart with items and totals, or null if no cart exists</returns>
    Task<Result<CartResponse?>> GetCartByUserIdAsync(string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Add or increment an item in the user's cart
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="request">Add cart item request with ProductId and Quantity</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated cart</returns>
    /// <remarks>
    /// Business Rules:
    /// - Product must exist and be active (IsActive = true)
    /// - Requested quantity must be > 0
    /// - Total quantity (existing + new) cannot exceed inventory
    /// - If product already in cart, quantities are summed
    /// - UnitPrice is captured from current product price
    /// </remarks>
    Task<Result<CartResponse>> AddToCartAsync(string userId, AddCartItemRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update quantity of an item in the cart
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="productId">Product ID in the cart</param>
    /// <param name="request">Update request with new quantity</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated cart</returns>
    /// <remarks>
    /// Business Rules:
    /// - Quantity must be >= 0
    /// - Setting quantity to 0 removes the item from cart
    /// - Updated quantity cannot exceed available inventory
    /// </remarks>
    Task<Result<CartResponse>> UpdateCartItemAsync(string userId, int productId, UpdateCartItemRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove a specific item from the user's cart
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="productId">Product ID to remove from cart</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated cart after removal</returns>
    Task<Result<CartResponse>> RemoveFromCartAsync(string userId, int productId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Clear all items from user's cart
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result success/failure</returns>
    Task<Result> ClearCartAsync(string userId, CancellationToken cancellationToken = default);
}
