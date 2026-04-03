using IplMerchStore.Application.Common;
using IplMerchStore.Application.DTOs;

namespace IplMerchStore.Application.Interfaces;

/// <summary>
/// Service interface for product search functionality
/// 
/// Design Note: This interface uses IQueryable composition which allows for:
/// 1. Efficient filtering with EF Core / database-level queries
/// 2. Easy replacement with Azure AI Search implementation later
///    (simply swap the implementation while keeping the same interface)
/// 3. Interview-friendly architecture demonstrating separation of concerns
/// 
/// Future Integration (Azure AI Search):
/// To integrate Azure Cognitive Search:
/// - Create a new implementation of ISearchService (e.g., AzureSearchService)
/// - Keep the same interface contract for backward compatibility
/// - The SearchController and other consumers won't need changes
/// - Performance will improve due to cloud-based indexing and ranking
/// - Fuzzy matching and semantic search will be enhanced
/// </summary>
public interface ISearchService
{
    /// <summary>
    /// Search products with flexible filtering and pagination
    /// </summary>
    /// <param name="query">Text to search in product name and description (case-insensitive partial match)</param>
    /// <param name="franchiseId">Optional filter by franchise</param>
    /// <param name="productType">Optional filter by product type (1-8)</param>
    /// <param name="pageNumber">Page number for pagination</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated search results sorted by relevance then name</returns>
    Task<Result<PagedResult<ProductSearchResultDto>>> SearchProductsAsync(
        string? query = null,
        int? franchiseId = null,
        int? productType = null,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get search suggestions based on a partial query
    /// Returns top matching product names and franchise combinations
    /// for autocomplete/typeahead functionality
    /// </summary>
    /// <param name="query">Partial query text</param>
    /// <param name="franchiseId">Optional filter by franchise for franchise-specific suggestions</param>
    /// <param name="limit">Number of suggestions to return (default: 10, max: 50)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of suggestion strings</returns>
    Task<Result<IEnumerable<string>>> GetSearchSuggestionsAsync(
        string? query = null,
        int? franchiseId = null,
        int limit = 10,
        CancellationToken cancellationToken = default);
}
