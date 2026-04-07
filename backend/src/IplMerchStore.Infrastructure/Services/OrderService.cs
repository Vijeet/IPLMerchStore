using IplMerchStore.Application.Common;
using IplMerchStore.Application.DTOs;
using IplMerchStore.Application.Interfaces;
using IplMerchStore.Domain.Entities;
using IplMerchStore.Domain.Enums;
using IplMerchStore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IplMerchStore.Infrastructure.Services;

/// <summary>
/// Service for managing orders and checkout operations
/// 
/// Implementation Notes:
/// - Checkout validates cart, inventory, creates order and reduces inventory
/// - All checkout operations are transactional
/// - Cart is cleared after successful checkout
/// - Order items capture UnitPrice at time of order
/// - Order status is set to "Placed" after successful checkout
/// 
/// Business Rules Enforced:
/// 1. User cart must exist and contain at least one item
/// 2. All items in cart must have sufficient inventory
/// 3. Products must be active at time of checkout
/// 4. Inventory is reduced after order creation
/// 5. Cart is cleared after successful order
/// 6. Order status starts as "Placed"
/// </summary>
public class OrderService : IOrderService
{
    private readonly AppDbContext _dbContext;
    private readonly ICartService _cartService;
    private readonly IPaymentService _paymentService;
    private readonly ILogger<OrderService> _logger;

    public OrderService(
        AppDbContext dbContext,
        ICartService cartService,
        IPaymentService paymentService,
        ILogger<OrderService> logger)
    {
        _dbContext = dbContext;
        _cartService = cartService;
        _paymentService = paymentService;
        _logger = logger;
    }

    public async Task<Result<OrderDto?>> GetOrderByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (id <= 0)
            {
                return Result<OrderDto?>.FailureResult("Order ID must be greater than 0");
            }

            var order = await _dbContext.Orders
                .Where(o => o.Id == id)
                .Include(o => o.Items)
                .ThenInclude(oi => oi.Product)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            if (order == null)
            {
                return Result<OrderDto?>.SuccessResult(null, "Order not found");
            }

            var orderDto = MapOrderToDto(order);
            return Result<OrderDto?>.SuccessResult(orderDto, "Order retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving order with ID: {OrderId}", id);
            return Result<OrderDto?>.FailureResult("An error occurred while retrieving order");
        }
    }

    public async Task<Result<PagedResult<OrderDto>>> GetUserOrdersAsync(
        string userId,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Result<PagedResult<OrderDto>>.FailureResult("User ID is required");
            }

            if (pageNumber < 1)
            {
                return Result<PagedResult<OrderDto>>.FailureResult("Page number must be greater than 0");
            }

            if (pageSize < 1 || pageSize > 100)
            {
                return Result<PagedResult<OrderDto>>.FailureResult("Page size must be between 1 and 100");
            }

            // Query orders for user, ordered by latest first (descending by CreatedAtUtc)
            var query = _dbContext.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.Items)
                .ThenInclude(oi => oi.Product)
                .AsNoTracking()
                .OrderByDescending(o => o.CreatedAtUtc);

            // Get total count
            var totalCount = await query.CountAsync(cancellationToken);

            // Get paginated results
            var orders = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            // Map to DTOs
            var orderDtos = orders.Select(MapOrderToDto).ToList();

            var result = new PagedResult<OrderDto>(orderDtos, pageNumber, pageSize, totalCount);

            _logger.LogInformation(
                "Retrieved {OrderCount} orders for user {UserId}",
                orders.Count,
                userId);

            return Result<PagedResult<OrderDto>>.SuccessResult(result, "Orders retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving orders for userId: {UserId}", userId);
            return Result<PagedResult<OrderDto>>.FailureResult("An error occurred while retrieving orders");
        }
    }

    public async Task<Result<OrderDto>> CreateOrderAsync(
        string userId,
        CreateOrderDto orderDto,
        CancellationToken cancellationToken = default)
    {
        using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            // Validate userId
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Result<OrderDto>.FailureResult("User ID is required");
            }

            // Get user's cart
            var cart = await _dbContext.Carts
                .Where(c => c.UserId == userId)
                .Include(c => c.Items)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(cancellationToken);

            // Validate cart exists and is not empty
            if (cart == null || !cart.Items.Any())
            {
                return Result<OrderDto>.FailureResult("Cart is empty or does not exist. Cannot proceed with checkout.");
            }

            // Validate inventory for all items
            var inventoryValidationErrors = new List<string>();
            foreach (var cartItem in cart.Items)
            {
                var product = cartItem.Product;
                if (product == null)
                {
                    inventoryValidationErrors.Add($"Product information is missing for cart item");
                    continue;
                }

                if (!product.IsActive)
                {
                    inventoryValidationErrors.Add($"Product '{product.Name}' is no longer available");
                    continue;
                }

                if (cartItem.Quantity > product.InventoryCount)
                {
                    inventoryValidationErrors.Add(
                        $"Product '{product.Name}' has insufficient inventory. " +
                        $"Requested: {cartItem.Quantity}, Available: {product.InventoryCount}");
                }
            }

            if (inventoryValidationErrors.Any())
            {
                await transaction.RollbackAsync(cancellationToken);
                var errorMessage = string.Join("; ", inventoryValidationErrors);
                return Result<OrderDto>.FailureResult(errorMessage);
            }

            // Create order
            var now = DateTime.UtcNow;
            var order = new Order
            {
                Id = 0, // EF Core will generate the ID
                UserId = userId,
                Status = OrderStatus.Pending,
                ShippingAddress = orderDto.ShippingAddress,
                CustomerEmail = orderDto.CustomerEmail,
                CustomerPhone = orderDto.CustomerPhone,
                TotalAmount = 0, // Will be calculated below
                Items = new List<OrderItem>(),
                CreatedAtUtc = now,
                UpdatedAtUtc = now,
            };

            decimal orderTotal = 0;

            // Create order items and reduce inventory
            foreach (var cartItem in cart.Items)
            {
                var product = cartItem.Product!;

                // Create order item with current unit price from cart
                var orderItem = new OrderItem
                {Id = 0, // EF Core will generate the ID
                    
                    ProductId = product.Id,
                    Quantity = cartItem.Quantity,
                    UnitPrice = cartItem.UnitPrice,
                    SubTotal = cartItem.UnitPrice * cartItem.Quantity,
                    Product = product,
                    CreatedAtUtc = now,
                    UpdatedAtUtc = now,
                };

                order.Items.Add(orderItem);
                orderTotal += orderItem.SubTotal;

                // Reduce product inventory
                product.InventoryCount -= cartItem.Quantity;
            }

            // Set order total
            order.TotalAmount = orderTotal;

            // Add order to database and save all changes in one batch
            _dbContext.Orders.Add(order);
            await _dbContext.SaveChangesAsync(cancellationToken);

            // Process payment
            _logger.LogInformation("Processing payment for order {OrderId}, amount: {Amount}", order.Id, order.TotalAmount);
            var paymentResult = await _paymentService.ProcessPaymentAsync(order.Id, order.TotalAmount, "INR", cancellationToken);

            if (!paymentResult.Success)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError("Payment processing failed for order {OrderId}: {Message}", order.Id, paymentResult.Message);
                return Result<OrderDto>.FailureResult($"Payment processing failed: {paymentResult.Message}");
            }

            if (!paymentResult.Data)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError("Payment declined for order {OrderId}", order.Id);
                return Result<OrderDto>.FailureResult("Payment was declined. Please try again.");
            }

            _logger.LogInformation("Payment successful for order {OrderId}", order.Id);

            // Clear user's cart
            var clearCartResult = await _cartService.ClearCartAsync(userId, cancellationToken);
            if (!clearCartResult.Success)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError("Failed to clear cart for user {UserId} after order creation", userId);
                return Result<OrderDto>.FailureResult("Order created but failed to clear cart. Please contact support.");
            }

            // Commit transaction
            await transaction.CommitAsync(cancellationToken);

            // Map to DTO using the in-memory order (no need to reload from DB)
            var orderResponseDto = MapOrderToDto(order);

            _logger.LogInformation(
                "Order created successfully for user {UserId}. OrderId: {OrderId}, Total: {Total}",
                userId,
                order.Id,
                order.TotalAmount);

            return Result<OrderDto>.SuccessResult(
                orderResponseDto,
                "Order created successfully and cart has been cleared");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Error creating order for userId: {UserId}", userId);
            return Result<OrderDto>.FailureResult("An error occurred while creating order");
        }
    }

    /// <summary>
    /// Cancel a pending order. Only orders in Pending status can be cancelled.
    /// Restores product inventory for each order item.
    /// </summary>
    public async Task<Result<OrderDto>> CancelOrderAsync(string userId, int orderId, CancellationToken cancellationToken = default)
    {
        try
        {
            var order = await _dbContext.Orders
                .Include(o => o.Items)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);

            if (order == null || order.UserId != userId)
            {
                return Result<OrderDto>.FailureResult("Order not found");
            }

            if (order.Status != OrderStatus.Pending)
            {
                return Result<OrderDto>.FailureResult("Only pending orders can be cancelled");
            }

            // Restore inventory
            foreach (var item in order.Items)
            {
                if (item.Product != null)
                {
                    item.Product.InventoryCount += item.Quantity;
                }
            }

            order.Status = OrderStatus.Cancelled;
            order.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Order {OrderId} cancelled for user {UserId}", orderId, userId);
            return Result<OrderDto>.SuccessResult(MapOrderToDto(order), "Order cancelled successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling order {OrderId} for user {UserId}", orderId, userId);
            return Result<OrderDto>.FailureResult("An error occurred while cancelling the order");
        }
    }

    /// <summary>
    /// Maps an Order entity to OrderDto
    /// </summary>
    private static OrderDto MapOrderToDto(Order order)
    {
        var items = order.Items
            .Select(oi => new OrderItemDto
            {
                Id = oi.Id,
                ProductId = oi.ProductId,
                ProductName = oi.Product?.Name,
                Quantity = oi.Quantity,
                UnitPrice = oi.UnitPrice,
                SubTotal = oi.SubTotal
            })
            .ToList();

        return new OrderDto
        {
            Id = order.Id,
            UserId = order.UserId,
            TotalAmount = order.TotalAmount,
            Status = (int)order.Status,
            ShippingAddress = order.ShippingAddress,
            CustomerEmail = order.CustomerEmail,
            CustomerPhone = order.CustomerPhone,
            CreatedAtUtc = order.CreatedAtUtc,
            Items = items
        };
    }
}
