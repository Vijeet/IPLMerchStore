using IplMerchStore.Application.Common;
using IplMerchStore.Application.DTOs;

namespace IplMerchStore.Application.Interfaces;

/// <summary>
/// Service interface for managing products
/// </summary>
public interface IProductService
{
    /// <summary>
    /// Get all products with pagination and optional filtering
    /// </summary>
    Task<Result<PagedResult<ProductDto>>> GetAllProductsAsync(
        int pageNumber = 1, 
        int pageSize = 10,
        int? franchiseId = null,
        int? productType = null,
        bool? activeOnly = null,
        string? sortBy = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get a specific product by ID with detailed information
    /// </summary>
    Task<Result<ProductDetailDto?>> GetProductByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create a new product
    /// </summary>
    Task<Result<ProductDetailDto>> CreateProductAsync(ProductInputDto inputDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update an existing product
    /// </summary>
    Task<Result<ProductDetailDto>> UpdateProductAsync(int id, ProductInputDto inputDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a product (soft delete - marks as inactive)
    /// </summary>
    Task<Result> DeleteProductAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Search products by name
    /// </summary>
    Task<Result<PagedResult<ProductDto>>> SearchProductsAsync(string? name = null, int? franchiseId = null, int? productType = null, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default);
}

