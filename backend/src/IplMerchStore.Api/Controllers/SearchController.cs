using IplMerchStore.Application.DTOs;
using IplMerchStore.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IplMerchStore.Api.Controllers;

/// <summary>
/// Controller for product search operations
/// Provides endpoints for searching products and getting autocomplete suggestions
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class SearchController : ControllerBase
{
    private readonly ISearchService _searchService;

    public SearchController(ISearchService searchService)
    {
        _searchService = searchService;
    }

    /// <summary>
    /// Search products by query text and optional filters
    /// Searches across product names and descriptions
    /// Supports filtering by franchise and product type
    /// Results are sorted by relevance first, then alphabetically by product name
    /// </summary>
    /// <param name="q">Query text to search for (optional, matches product name or description)</param>
    /// <param name="franchiseId">Filter by franchise ID (optional)</param>
    /// <param name="type">Filter by product type 1-8 (optional): Jersey=1, Cap=2, Flag=3, AutographedPhoto=4, Mug=5, Hoodie=6, Keychain=7, Other=8</param>
    /// <param name="page">Page number for pagination (default: 1)</param>
    /// <param name="pageSize">Number of items per page (default: 10, max: 100)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated search results with relevance scores</returns>
    /// <example>
    /// GET /api/search?q=jersey&franchiseId=1&type=1&page=1&pageSize=10
    /// Returns all active products matching "jersey" from franchise 1 with type Jersey
    /// </example>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SearchProducts(
        [FromQuery(Name = "q")] string? q = null,
        [FromQuery] int? franchiseId = null,
        [FromQuery(Name = "type")] int? type = null,
        [FromQuery(Name = "page")] int page = 1,
        [FromQuery(Name = "pageSize")] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        // Validate pagination parameters
        if (page < 1 || pageSize < 1)
        {
            return BadRequest(new
            {
                success = false,
                message = "Page and pageSize must be greater than 0"
            });
        }

        if (pageSize > 100)
        {
            return BadRequest(new
            {
                success = false,
                message = "PageSize cannot exceed 100"
            });
        }

        // Validate type parameter if provided
        if (type.HasValue && (type.Value < 1 || type.Value > 8))
        {
            return BadRequest(new
            {
                success = false,
                message = "Product type must be between 1 and 8"
            });
        }

        var result = await _searchService.SearchProductsAsync(
            q,
            franchiseId,
            type,
            page,
            pageSize,
            cancellationToken);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Get search suggestions for autocomplete/typeahead functionality
    /// Returns top matching product names with franchise associations
    /// Useful for building search interfaces with instant suggestions
    /// </summary>
    /// <param name="q">Partial query text for matching suggestions (optional)</param>
    /// <param name="franchiseId">Filter suggestions by franchise ID (optional)</param>
    /// <param name="limit">Number of suggestions to return (default: 10, max: 50)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of suggestion strings in format "Product Name (FRANCHISE_CODE)"</returns>
    /// <example>
    /// GET /api/search/suggestions?q=jer&limit=5
    /// Returns up to 5 product name suggestions containing "jer"
    /// </example>
    [HttpGet("suggestions")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetSearchSuggestions(
        [FromQuery(Name = "q")] string? q = null,
        [FromQuery] int? franchiseId = null,
        [FromQuery] int limit = 10,
        CancellationToken cancellationToken = default)
    {
        // Validate limit parameter
        if (limit < 1)
        {
            return BadRequest(new
            {
                success = false,
                message = "Limit must be greater than 0"
            });
        }

        if (limit > 50)
        {
            return BadRequest(new
            {
                success = false,
                message = "Limit cannot exceed 50"
            });
        }

        var result = await _searchService.GetSearchSuggestionsAsync(
            q,
            franchiseId,
            limit,
            cancellationToken);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}
