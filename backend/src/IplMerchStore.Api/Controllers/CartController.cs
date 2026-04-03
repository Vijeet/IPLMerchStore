using IplMerchStore.Application.DTOs;
using IplMerchStore.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IplMerchStore.Api.Controllers;

/// <summary>
/// Controller for managing user shopping carts
/// 
/// No authentication required - userId is passed in route for demo purposes
/// Each user has one active cart that persists across requests
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;
    private readonly ILogger<CartController> _logger;

    public CartController(ICartService cartService, ILogger<CartController> logger)
    {
        _cartService = cartService;
        _logger = logger;
    }

    /// <summary>
    /// Get cart for a specific user
    /// Returns empty cart state cleanly if no items (not null/error)
    /// </summary>
    /// <param name="userId">User identifier (no authentication required)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User's cart with items and totals</returns>
    /// <remarks>
    /// Returns 404 if user has never had a cart
    /// Returns 200 with CartResponse if cart exists (may be empty)
    /// </remarks>
    [HttpGet("{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CartResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetCart(string userId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return BadRequest("User ID is required");
        }

        var result = await _cartService.GetCartByUserIdAsync(userId, cancellationToken);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        if (result.Data == null)
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Add an item to user's cart (or increment quantity if already present)
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="request">Request with ProductId and Quantity to add</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated cart</returns>
    /// <remarks>
    /// Product must exist and be active
    /// Quantity must be > 0
    /// Total quantity cannot exceed available inventory
    /// If product already in cart, quantities are summed
    /// </remarks>
    [HttpPost("{userId}/items")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CartResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddToCart(
        string userId,
        [FromBody] AddCartItemRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return BadRequest("User ID is required");
        }

        if (request.ProductId <= 0)
        {
            return BadRequest("Product ID must be greater than 0");
        }

        if (request.Quantity <= 0)
        {
            return BadRequest("Quantity must be greater than 0");
        }

        var result = await _cartService.AddToCartAsync(userId, request, cancellationToken);

        if (!result.Success)
        {
            // Return 404 if product not found, 400 for validation errors
            if (result.Message?.Contains("not found") == true)
            {
                return NotFound(result);
            }

            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Update quantity of an item in user's cart
    /// Setting quantity to 0 removes the item
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="productId">Product ID to update in cart</param>
    /// <param name="request">Request with new quantity</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated cart</returns>
    /// <remarks>
    /// Quantity must be >= 0
    /// Setting quantity to 0 removes the item from cart
    /// New quantity cannot exceed available inventory
    /// </remarks>
    [HttpPut("{userId}/items/{productId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CartResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCartItem(
        string userId,
        int productId,
        [FromBody] UpdateCartItemRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return BadRequest("User ID is required");
        }

        if (productId <= 0)
        {
            return BadRequest("Product ID must be greater than 0");
        }

        if (request.Quantity < 0)
        {
            return BadRequest("Quantity cannot be negative");
        }

        var result = await _cartService.UpdateCartItemAsync(userId, productId, request, cancellationToken);

        if (!result.Success)
        {
            // Return 404 if cart or item not found, 400 for validation errors
            if (result.Message?.Contains("not found") == true)
            {
                return NotFound(result);
            }

            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Remove a specific item from user's cart
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="productId">Product ID to remove from cart</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated cart</returns>
    [HttpDelete("{userId}/items/{productId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CartResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveFromCart(
        string userId,
        int productId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return BadRequest("User ID is required");
        }

        if (productId <= 0)
        {
            return BadRequest("Product ID must be greater than 0");
        }

        var result = await _cartService.RemoveFromCartAsync(userId, productId, cancellationToken);

        if (!result.Success)
        {
            if (result.Message?.Contains("not found") == true)
            {
                return NotFound(result);
            }

            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Clear all items from user's cart
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success result</returns>
    [HttpDelete("{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ClearCart(string userId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return BadRequest("User ID is required");
        }

        var result = await _cartService.ClearCartAsync(userId, cancellationToken);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}
