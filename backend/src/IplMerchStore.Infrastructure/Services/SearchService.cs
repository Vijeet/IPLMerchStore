using IplMerchStore.Application.Common;
using IplMerchStore.Application.DTOs;
using IplMerchStore.Application.Interfaces;
using IplMerchStore.Domain.Enums;
using IplMerchStore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IplMerchStore.Infrastructure.Services;

/// <summary>
/// Service for searching products with flexible filtering and pagination
/// 
/// Implementation Notes:
/// - Uses IQueryable composition for efficient database queries
/// - Search is performed case-insensitively on product name and description
/// - Results are ranked by relevance (name matches score higher than description matches)
/// - Then sorted alphabetically by product name
/// - Supports pagination for large result sets
/// 
/// Azure AI Search Migration:
/// To migrate to Azure Cognitive Search, create AzureSearchService implementing ISearchService:
/// 1. Initialize Azure SDK client in constructor
/// 2. In SearchProductsAsync: Build Azure search query with filters, execute, map results
/// 3. In GetSearchSuggestionsAsync: Use Azure suggestions API
/// 4. Update Program.cs DI registration to use new implementation
/// 5. No changes needed in controllers or callers - interface remains the same
/// 
/// Performance Considerations:
/// - EF Core queries are translated to SQL for database execution
/// - Partial text matching uses SQL's LIKE operator (database-level filtering)
/// - Large result sets benefit from pagination
/// - Current limit: up to 100 items per page
/// - For production with 100k+ products, Azure Search would be recommended
/// </summary>
public class SearchService : ISearchService
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<SearchService> _logger;
    private const int MaxPageSize = 100;

    public SearchService(AppDbContext dbContext, ILogger<SearchService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<PagedResult<ProductSearchResultDto>>> SearchProductsAsync(
        string? query = null,
        int? franchiseId = null,
        int? productType = null,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate pagination parameters
            if (pageNumber < 1)
                pageNumber = 1;
            if (pageSize < 1)
                pageSize = 10;
            if (pageSize > MaxPageSize)
                pageSize = MaxPageSize;

            // Start with base query including related franchise data
            var searchQuery = _dbContext.Products
                .Include(p => p.Franchise)
                .AsQueryable();

            // Apply text search filter on product name and description
            // This is database-level filtering for performance
            if (!string.IsNullOrWhiteSpace(query))
            {
                var normalizedQuery = query.Trim().ToLower();
                searchQuery = searchQuery.Where(p =>
                    p.Name.ToLower().Contains(normalizedQuery) ||
                    p.Description.ToLower().Contains(normalizedQuery));
            }

            // Apply franchise filter
            if (franchiseId.HasValue && franchiseId.Value > 0)
            {
                searchQuery = searchQuery.Where(p => p.FranchiseId == franchiseId.Value);
            }

            // Apply product type filter
            if (productType.HasValue && productType.Value > 0)
            {
                searchQuery = searchQuery.Where(p => (int)p.ProductType == productType.Value);
            }

            // Only return active products by default (search results show available products)
            searchQuery = searchQuery.Where(p => p.IsActive);

            // Get total count before pagination
            var totalCount = await searchQuery.CountAsync(cancellationToken);

            // Calculate relevance score and apply sorting
            // Name matches score higher (90-100) than description matches (50-89)
            // Then sort alphabetically by name for consistent results
            var results = searchQuery
                .AsEnumerable() // Switch to LINQ-to-Objects for relevance calculation
                .Select(p => new
                {
                    Product = p,
                    RelevanceScore = CalculateRelevanceScore(p, query)
                })
                .OrderByDescending(x => x.RelevanceScore)
                .ThenBy(x => x.Product.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => MapToProductSearchResultDto(x.Product, x.RelevanceScore))
                .ToList();

            var pagedResult = new PagedResult<ProductSearchResultDto>(
                results,
                pageNumber,
                pageSize,
                totalCount);

            return Result<PagedResult<ProductSearchResultDto>>.SuccessResult(
                pagedResult,
                $"Found {totalCount} product(s) matching your search criteria");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching products with query: {Query}, franchiseId: {FranchiseId}, productType: {ProductType}",
                query, franchiseId, productType);
            return Result<PagedResult<ProductSearchResultDto>>.FailureResult(
                $"Failed to search products: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<string>>> GetSearchSuggestionsAsync(
        string? query = null,
        int? franchiseId = null,
        int limit = 10,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate limit parameter
            if (limit < 1)
                limit = 1;
            if (limit > 50)
                limit = 50;

            var suggestionsQuery = _dbContext.Products
                .Where(p => p.IsActive)
                .AsQueryable();

            // Apply franchise filter if specified
            if (franchiseId.HasValue && franchiseId.Value > 0)
            {
                suggestionsQuery = suggestionsQuery.Where(p => p.FranchiseId == franchiseId.Value);
            }

            // Filter by query text if provided
            if (!string.IsNullOrWhiteSpace(query))
            {
                var normalizedQuery = query.Trim().ToLower();
                suggestionsQuery = suggestionsQuery.Where(p =>
                    p.Name.ToLower().Contains(normalizedQuery) ||
                    p.Description.ToLower().Contains(normalizedQuery));
            }

            // Get unique product names and franchise combinations
            // Sorted by relevance (name matches first) then alphabetically
            var suggestions = suggestionsQuery
                .AsEnumerable()
                .Select(p => new
                {
                    Product = p,
                    RelevanceScore = CalculateRelevanceScore(p, query)
                })
                .OrderByDescending(x => x.RelevanceScore)
                .ThenBy(x => x.Product.Name)
                .Select(x => x.Product)
                .Take(limit)
                .Select(p => $"{p.Name} ({p.Franchise?.ShortCode ?? "Unknown"})")
                .ToList();

            return Result<IEnumerable<string>>.SuccessResult(
                suggestions,
                $"Retrieved {suggestions.Count} search suggestions");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting search suggestions for query: {Query}, franchiseId: {FranchiseId}",
                query, franchiseId);
            return Result<IEnumerable<string>>.FailureResult(
                $"Failed to get search suggestions: {ex.Message}");
        }
    }

    /// <summary>
    /// Calculate a relevance score for a product based on the search query
    /// This method demonstrates how relevance ranking works with EF Core
    /// 
    /// Scoring Logic:
    /// - Name match (starts with): 100 (most relevant)
    /// - Name match (contains): 90 (very relevant)
    /// - Description match: 50 (less relevant)
    /// 
    /// When migrating to Azure Search:
    /// - Azure's ranking algorithm can be used instead
    /// - Can incorporate BM25 scoring and semantic understanding
    /// - Can weight different fields differently
    /// </summary>
    private int CalculateRelevanceScore(Domain.Entities.Product product, string? query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return 50; // Neutral score if no query

        var normalizedQuery = query.Trim().ToLower();
        var productName = product.Name?.ToLower() ?? string.Empty;
        var productDescription = product.Description?.ToLower() ?? string.Empty;

        // Highest score: product name starts with query
        if (productName.StartsWith(normalizedQuery))
            return 100;

        // Very high score: product name contains query
        if (productName.Contains(normalizedQuery))
            return 90;

        // Medium score: description contains query
        if (productDescription.Contains(normalizedQuery))
            return 50;

        // Fallback: partial word match (e.g., "jer" matches "jersey")
        return 30;
    }

    /// <summary>
    /// Map a Product entity to ProductSearchResultDto
    /// Includes relevance score for sorted results
    /// </summary>
    private ProductSearchResultDto MapToProductSearchResultDto(
        Domain.Entities.Product product,
        int relevanceScore)
    {
        return new ProductSearchResultDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Currency = product.Currency,
            InventoryCount = product.InventoryCount,
            ProductType = (int)product.ProductType,
            ProductTypeLabel = product.ProductType.ToString(),
            FranchiseId = product.FranchiseId,
            FranchiseName = product.Franchise?.Name,
            ImageUrl = product.ImageUrl,
            SKU = product.SKU,
            IsActive = product.IsActive,
            RelevanceScore = relevanceScore,
            CreatedAtUtc = product.CreatedAtUtc
        };
    }
}
