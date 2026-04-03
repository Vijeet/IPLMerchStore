using IplMerchStore.Application.Common;
using IplMerchStore.Application.DTOs;
using IplMerchStore.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IplMerchStore.Api.Controllers;

/// <summary>
/// Controller for managing orders and checkout operations
/// 
/// Endpoints:
/// - POST /api/orders/checkout - Create order from cart
/// - GET /api/orders/{userId} - Get user's order history
/// - GET /api/orders/{userId}/{orderId} - Get specific order details
/// 
/// No authentication required - userId is passed in route for demo purposes
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(IOrderService orderService, ILogger<OrdersController> logger)
    {
        _orderService = orderService;
        _logger = logger;
    }

    /// <summary>
    /// Create an order from the user's cart (checkout operation)
    /// 
    /// Process:
    /// 1. Validates user cart exists and is not empty
    /// 2. Validates all items have sufficient inventory
    /// 3. Creates Order and OrderItems with captured unit prices
    /// 4. Reduces product inventory
    /// 5. Clears user's cart
    /// 6. Returns created Order with status "Placed"
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="checkoutRequest">Checkout details (shipping address, email, phone)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created order details</returns>
    /// <remarks>
    /// Returns 400 if:
    /// - User ID is invalid
    /// - Cart is empty or doesn't exist
    /// - Any item has insufficient inventory
    /// 
    /// Returns 200 with OrderDto on success
    /// </remarks>
    [HttpPost("checkout")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OrderDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Checkout(
        [FromQuery] string userId,
        [FromBody] CheckoutRequestDto checkoutRequest,
        CancellationToken cancellationToken = default)
    {
        // Validate userId
        if (string.IsNullOrWhiteSpace(userId))
        {
            return BadRequest("User ID is required");
        }

        _logger.LogInformation("Checkout initiated for user {UserId}", userId);

        // Call service to create order
        var createOrderDto = new CreateOrderDto
        {
            ShippingAddress = checkoutRequest.ShippingAddress,
            CustomerEmail = checkoutRequest.CustomerEmail,
            CustomerPhone = checkoutRequest.CustomerPhone
        };

        var result = await _orderService.CreateOrderAsync(userId, createOrderDto, cancellationToken);

        if (!result.Success)
        {
            _logger.LogWarning("Checkout failed for user {UserId}: {Message}", userId, result.Message);
            return BadRequest(result.Message);
        }

        _logger.LogInformation("Checkout successful for user {UserId}, OrderId: {OrderId}", userId, result.Data?.Id);
        return Ok(result.Data);
    }

    /// <summary>
    /// Get all orders for a user, sorted by latest first
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Items per page (default: 10, max: 100)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of user's orders</returns>
    /// <remarks>
    /// Returns 400 if:
    /// - User ID is invalid
    /// - Page parameters are invalid
    /// 
    /// Returns 200 with PagedResult of OrderDto on success
    /// </remarks>
    [HttpGet("{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedResult<OrderDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUserOrders(
        string userId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        // Validate userId
        if (string.IsNullOrWhiteSpace(userId))
        {
            return BadRequest("User ID is required");
        }

        _logger.LogInformation("Retrieving orders for user {UserId}, page {PageNumber} size {PageSize}",
            userId, pageNumber, pageSize);

        var result = await _orderService.GetUserOrdersAsync(userId, pageNumber, pageSize, cancellationToken);

        if (!result.Success)
        {
            _logger.LogWarning("Failed to retrieve orders for user {UserId}: {Message}", userId, result.Message);
            return BadRequest(result.Message);
        }

        if (result.Data == null || !result.Data.Items.Any())
        {
            _logger.LogInformation("No orders found for user {UserId}", userId);
            return Ok(result.Data);
        }

        _logger.LogInformation("Retrieved {OrderCount} orders for user {UserId}", result.Data.Items.Count(), userId);
        return Ok(result.Data);
    }

    /// <summary>
    /// Get details of a specific order including all items
    /// </summary>
    /// <param name="userId">User identifier (for validation)</param>
    /// <param name="orderId">Order identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Detailed order information with all items</returns>
    /// <remarks>
    /// Returns 400 if:
    /// - User ID or Order ID is invalid
    /// 
    /// Returns 404 if order not found
    /// 
    /// Returns 200 with OrderDto on success
    /// </remarks>
    [HttpGet("{userId}/{orderId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OrderDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetOrderDetail(
        string userId,
        int orderId,
        CancellationToken cancellationToken = default)
    {
        // Validate inputs
        if (string.IsNullOrWhiteSpace(userId))
        {
            return BadRequest("User ID is required");
        }

        if (orderId <= 0)
        {
            return BadRequest("Order ID must be greater than 0");
        }

        _logger.LogInformation("Retrieving order details for orderId {OrderId} for user {UserId}",
            orderId, userId);

        var result = await _orderService.GetOrderByIdAsync(orderId, cancellationToken);

        if (!result.Success)
        {
            _logger.LogWarning("Failed to retrieve order {OrderId}: {Message}", orderId, result.Message);
            return BadRequest(result.Message);
        }

        if (result.Data == null)
        {
            _logger.LogInformation("Order {OrderId} not found", orderId);
            return NotFound(new { message = "Order not found" });
        }

        // Verify order belongs to the requesting user
        if (result.Data.UserId != userId)
        {
            _logger.LogWarning("User {UserId} attempted to access order {OrderId} belonging to {ActualUserId}",
                userId, orderId, result.Data.UserId);
            return NotFound(new { message = "Order not found" });
        }

        _logger.LogInformation("Retrieved order {OrderId} for user {UserId}", orderId, userId);
        return Ok(result.Data);
    }
}
