using IplMerchStore.Application.Common;
using IplMerchStore.Application.DTOs;

namespace IplMerchStore.Application.Interfaces;

/// <summary>
/// Service interface for managing products
/// </summary>
public interface IProductService
{
    Task<Result<PagedResult<ProductDto>>> GetProductsAsync(int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default);
    Task<Result<ProductDto?>> GetProductByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<ProductDto>>> SearchProductsAsync(string? name = null, int? franchiseId = null, int? productType = null, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default);
}
