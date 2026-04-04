using IplMerchStore.Application.Common;
using IplMerchStore.Application.DTOs;

namespace IplMerchStore.Application.Interfaces;

/// <summary>
/// Service interface for managing orders
/// </summary>
public interface IOrderService
{
    Task<Result<OrderDto?>> GetOrderByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<OrderDto>>> GetUserOrdersAsync(string userId, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default);
    Task<Result<OrderDto>> CreateOrderAsync(string userId, CreateOrderDto orderDto, CancellationToken cancellationToken = default);
    Task<Result<OrderDto>> CancelOrderAsync(string userId, int orderId, CancellationToken cancellationToken = default);
}
