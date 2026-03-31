using IplMerchStore.Application.Common;
using IplMerchStore.Application.DTOs;

namespace IplMerchStore.Application.Interfaces;

/// <summary>
/// Service interface for managing carts
/// </summary>
public interface ICartService
{
    Task<Result<CartDto?>> GetCartByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<Result<CartDto>> AddToCartAsync(string userId, int productId, int quantity, CancellationToken cancellationToken = default);
    Task<Result<CartDto>> RemoveFromCartAsync(string userId, int cartItemId, CancellationToken cancellationToken = default);
    Task<Result> ClearCartAsync(string userId, CancellationToken cancellationToken = default);
}
