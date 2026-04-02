using IplMerchStore.Application.Common;
using IplMerchStore.Application.DTOs;
using IplMerchStore.Application.Interfaces;
using IplMerchStore.Application.Validators;
using IplMerchStore.Domain.Entities;
using IplMerchStore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IplMerchStore.Infrastructure.Services;

/// <summary>
/// Service for managing products
/// </summary>
public class ProductService : IProductService
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<ProductService> _logger;

    public ProductService(AppDbContext dbContext, ILogger<ProductService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<PagedResult<ProductDto>>> GetAllProductsAsync(
        int pageNumber = 1,
        int pageSize = 10,
        int? franchiseId = null,
        int? productType = null,
        bool? activeOnly = null,
        string? sortBy = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = _dbContext.Products
                .Include(p => p.Franchise)
                .AsQueryable();

            // Apply filters
            if (franchiseId.HasValue && franchiseId.Value > 0)
            {
                query = query.Where(p => p.FranchiseId == franchiseId.Value);
            }

            if (productType.HasValue)
            {
                query = query.Where(p => (int)p.ProductType == productType.Value);
            }

            if (activeOnly.HasValue && activeOnly.Value)
            {
                query = query.Where(p => p.IsActive);
            }

            // Get total count before pagination
            var totalCount = await query.CountAsync(cancellationToken);

            // Apply sorting
            query = sortBy?.ToLowerInvariant() switch
            {
                "name" => query.OrderBy(p => p.Name),
                "name_desc" => query.OrderByDescending(p => p.Name),
                "price" => query.OrderBy(p => p.Price),
                "price_desc" => query.OrderByDescending(p => p.Price),
                _ => query.OrderBy(p => p.CreatedAtUtc).ThenBy(p => p.Name) // Default: by creation date then name
            };

            // Apply pagination
            var products = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    Currency = p.Currency,
                    InventoryCount = p.InventoryCount,
                    ProductType = (int)p.ProductType,
                    FranchiseId = p.FranchiseId,
                    FranchiseName = p.Franchise!.Name,
                    ImageUrl = p.ImageUrl,
                    IsActive = p.IsActive,
                    SKU = p.SKU,
                    CreatedAtUtc = p.CreatedAtUtc,
                    UpdatedAtUtc = p.UpdatedAtUtc
                })
                .ToListAsync(cancellationToken);

            var pagedResult = new PagedResult<ProductDto>(products, pageNumber, pageSize, totalCount);
            return Result<PagedResult<ProductDto>>.SuccessResult(pagedResult, "Products retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving products");
            return Result<PagedResult<ProductDto>>.FailureResult("Failed to retrieve products");
        }
    }

    public async Task<Result<ProductDetailDto?>> GetProductByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var product = await _dbContext.Products
                .Include(p => p.Franchise)
                .Where(p => p.Id == id)
                .Select(p => new ProductDetailDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    Currency = p.Currency,
                    InventoryCount = p.InventoryCount,
                    ProductType = p.ProductType.ToString(),
                    FranchiseId = p.FranchiseId,
                    FranchiseName = p.Franchise!.Name,
                    FranchiseShortCode = p.Franchise!.ShortCode,
                    ImageUrl = p.ImageUrl,
                    IsActive = p.IsActive,
                    SKU = p.SKU,
                    CreatedAtUtc = p.CreatedAtUtc,
                    UpdatedAtUtc = p.UpdatedAtUtc
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (product == null)
            {
                return Result<ProductDetailDto?>.FailureResult($"Product with ID {id} not found");
            }

            return Result<ProductDetailDto?>.SuccessResult(product, "Product retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving product with ID {ProductId}", id);
            return Result<ProductDetailDto?>.FailureResult("Failed to retrieve product");
        }
    }

    public async Task<Result<ProductDetailDto>> CreateProductAsync(ProductInputDto inputDto, CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate input
            var (isValid, validationErrors) = ProductValidator.ValidateProductInputDto(inputDto);
            if (!isValid)
            {
                return Result<ProductDetailDto>.FailureResult("Validation failed", validationErrors);
            }

            // Check if franchise exists
            var franchiseExists = await _dbContext.Franchises
                .AnyAsync(f => f.Id == inputDto.FranchiseId, cancellationToken);
            if (!franchiseExists)
            {
                return Result<ProductDetailDto>.FailureResult("Validation failed", 
                    new[] { "Specified franchise does not exist" });
            }

            // Check for duplicate SKU
            var skuExists = await _dbContext.Products
                .AnyAsync(p => p.SKU.ToLower() == inputDto.SKU.ToLower(), cancellationToken);
            if (skuExists)
            {
                return Result<ProductDetailDto>.FailureResult("Validation failed", 
                    new[] { "A product with this SKU already exists" });
            }

            var now = DateTime.UtcNow;
            var product = new Product
            {
                Id = 0, // Will be generated by database
                Name = inputDto.Name,
                Description = inputDto.Description,
                Price = inputDto.Price,
                Currency = inputDto.Currency ?? "INR",
                InventoryCount = inputDto.InventoryCount,
                ProductType = (Domain.Enums.ProductType)inputDto.ProductType,
                FranchiseId = inputDto.FranchiseId,
                ImageUrl = inputDto.ImageUrl,
                IsActive = inputDto.IsActive,
                SKU = inputDto.SKU.ToUpperInvariant(),
                CreatedAtUtc = now,
                UpdatedAtUtc = now
            };

            _dbContext.Products.Add(product);
            await _dbContext.SaveChangesAsync(cancellationToken);

            // Fetch the franchise name for the response
            var franchise = await _dbContext.Franchises
                .Where(f => f.Id == product.FranchiseId)
                .Select(f => new { f.Name, f.ShortCode })
                .FirstOrDefaultAsync(cancellationToken);

            var productDetailDto = new ProductDetailDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Currency = product.Currency,
                InventoryCount = product.InventoryCount,
                ProductType = product.ProductType.ToString(),
                FranchiseId = product.FranchiseId,
                FranchiseName = franchise?.Name,
                FranchiseShortCode = franchise?.ShortCode,
                ImageUrl = product.ImageUrl,
                IsActive = product.IsActive,
                SKU = product.SKU,
                CreatedAtUtc = product.CreatedAtUtc,
                UpdatedAtUtc = product.UpdatedAtUtc
            };

            return Result<ProductDetailDto>.SuccessResult(productDetailDto, "Product created successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product");
            return Result<ProductDetailDto>.FailureResult("Failed to create product");
        }
    }

    public async Task<Result<ProductDetailDto>> UpdateProductAsync(int id, ProductInputDto inputDto, CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate input
            var (isValid, validationErrors) = ProductValidator.ValidateProductInputDto(inputDto);
            if (!isValid)
            {
                return Result<ProductDetailDto>.FailureResult("Validation failed", validationErrors);
            }

            var product = await _dbContext.Products.FindAsync(new object[] { id }, cancellationToken);
            if (product == null)
            {
                return Result<ProductDetailDto>.FailureResult($"Product with ID {id} not found");
            }

            // Check if franchise exists
            if (inputDto.FranchiseId != product.FranchiseId)
            {
                var franchiseExists = await _dbContext.Franchises
                    .AnyAsync(f => f.Id == inputDto.FranchiseId, cancellationToken);
                if (!franchiseExists)
                {
                    return Result<ProductDetailDto>.FailureResult("Validation failed", 
                        new[] { "Specified franchise does not exist" });
                }
            }

            // Check for duplicate SKU (excluding current product)
            var skuExists = await _dbContext.Products
                .AnyAsync(p => p.SKU.ToLower() == inputDto.SKU.ToLower() && p.Id != id, cancellationToken);
            if (skuExists)
            {
                return Result<ProductDetailDto>.FailureResult("Validation failed", 
                    new[] { "A product with this SKU already exists" });
            }

            product.Name = inputDto.Name;
            product.Description = inputDto.Description;
            product.Price = inputDto.Price;
            product.Currency = inputDto.Currency ?? "INR";
            product.InventoryCount = inputDto.InventoryCount;
            product.ProductType = (Domain.Enums.ProductType)inputDto.ProductType;
            product.FranchiseId = inputDto.FranchiseId;
            product.ImageUrl = inputDto.ImageUrl;
            product.IsActive = inputDto.IsActive;
            product.SKU = inputDto.SKU.ToUpperInvariant();
            product.UpdatedAtUtc = DateTime.UtcNow;

            _dbContext.Products.Update(product);
            await _dbContext.SaveChangesAsync(cancellationToken);

            // Fetch the franchise name for the response
            var franchise = await _dbContext.Franchises
                .Where(f => f.Id == product.FranchiseId)
                .Select(f => new { f.Name, f.ShortCode })
                .FirstOrDefaultAsync(cancellationToken);

            var productDetailDto = new ProductDetailDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Currency = product.Currency,
                InventoryCount = product.InventoryCount,
                ProductType = product.ProductType.ToString(),
                FranchiseId = product.FranchiseId,
                FranchiseName = franchise?.Name,
                FranchiseShortCode = franchise?.ShortCode,
                ImageUrl = product.ImageUrl,
                IsActive = product.IsActive,
                SKU = product.SKU,
                CreatedAtUtc = product.CreatedAtUtc,
                UpdatedAtUtc = product.UpdatedAtUtc
            };

            return Result<ProductDetailDto>.SuccessResult(productDetailDto, "Product updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating product with ID {ProductId}", id);
            return Result<ProductDetailDto>.FailureResult("Failed to update product");
        }
    }

    public async Task<Result> DeleteProductAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var product = await _dbContext.Products.FindAsync(new object[] { id }, cancellationToken);
            if (product == null)
            {
                return Result.FailureResult($"Product with ID {id} not found");
            }

            // Soft delete: mark as inactive instead of hard delete
            product.IsActive = false;
            product.UpdatedAtUtc = DateTime.UtcNow;

            _dbContext.Products.Update(product);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.SuccessResult("Product deleted successfully (marked as inactive)");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting product with ID {ProductId}", id);
            return Result.FailureResult("Failed to delete product");
        }
    }

    public async Task<Result<PagedResult<ProductDto>>> SearchProductsAsync(
        string? name = null,
        int? franchiseId = null,
        int? productType = null,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = _dbContext.Products
                .Include(p => p.Franchise)
                .AsQueryable();

            // Apply name filter
            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(p => p.Name.ToLower().Contains(name.ToLower()) || 
                                         p.Description.ToLower().Contains(name.ToLower()));
            }

            // Apply franchise filter
            if (franchiseId.HasValue && franchiseId.Value > 0)
            {
                query = query.Where(p => p.FranchiseId == franchiseId.Value);
            }

            // Apply product type filter
            if (productType.HasValue)
            {
                query = query.Where(p => (int)p.ProductType == productType.Value);
            }

            // Get total count before pagination
            var totalCount = await query.CountAsync(cancellationToken);

            // Apply pagination
            var products = await query
                .OrderBy(p => p.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    Currency = p.Currency,
                    InventoryCount = p.InventoryCount,
                    ProductType = (int)p.ProductType,
                    FranchiseId = p.FranchiseId,
                    FranchiseName = p.Franchise!.Name,
                    ImageUrl = p.ImageUrl,
                    IsActive = p.IsActive,
                    SKU = p.SKU,
                    CreatedAtUtc = p.CreatedAtUtc,
                    UpdatedAtUtc = p.UpdatedAtUtc
                })
                .ToListAsync(cancellationToken);

            var pagedResult = new PagedResult<ProductDto>(products, pageNumber, pageSize, totalCount);
            return Result<PagedResult<ProductDto>>.SuccessResult(pagedResult, "Products searched successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching products");
            return Result<PagedResult<ProductDto>>.FailureResult("Failed to search products");
        }
    }
}
