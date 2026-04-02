using IplMerchStore.Application.DTOs;
using IplMerchStore.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IplMerchStore.Api.Controllers;

/// <summary>
/// Controller for managing products
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    /// <summary>
    /// Get all products with pagination and optional filtering
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Number of items per page (default: 10)</param>
    /// <param name="franchiseId">Optional: Filter by franchise ID</param>
    /// <param name="productType">Optional: Filter by product type (1-8)</param>
    /// <param name="activeOnly">Optional: Show only active products (true/false)</param>
    /// <param name="sortBy">Optional: Sort by name, name_desc, price, or price_desc</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of products</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetProducts(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] int? franchiseId = null,
        [FromQuery] int? productType = null,
        [FromQuery] bool? activeOnly = null,
        [FromQuery] string? sortBy = null,
        CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1 || pageSize < 1)
        {
            return BadRequest("Page number and page size must be greater than 0");
        }

        var result = await _productService.GetAllProductsAsync(
            pageNumber,
            pageSize,
            franchiseId,
            productType,
            activeOnly,
            sortBy,
            cancellationToken);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Get a specific product by ID with detailed information including franchise details
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Detailed product information</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProductById(int id, CancellationToken cancellationToken = default)
    {
        var result = await _productService.GetProductByIdAsync(id, cancellationToken);

        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Create a new product
    /// </summary>
    /// <param name="inputDto">Product creation data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created product with assigned ID</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateProduct(ProductInputDto inputDto, CancellationToken cancellationToken = default)
    {
        var result = await _productService.CreateProductAsync(inputDto, cancellationToken);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return CreatedAtAction(nameof(GetProductById), new { id = result.Data?.Id }, result);
    }

    /// <summary>
    /// Update an existing product
    /// </summary>
    /// <param name="id">Product ID to update</param>
    /// <param name="inputDto">Updated product data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated product information</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProduct(int id, ProductInputDto inputDto, CancellationToken cancellationToken = default)
    {
        var result = await _productService.UpdateProductAsync(id, inputDto, cancellationToken);

        if (!result.Success)
        {
            // Check if it's a not found error
            if (result.Message?.Contains("not found") == true)
            {
                return NotFound(result);
            }
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Delete a product (soft delete - marks as inactive)
    /// </summary>
    /// <param name="id">Product ID to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Deletion confirmation</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProduct(int id, CancellationToken cancellationToken = default)
    {
        var result = await _productService.DeleteProductAsync(id, cancellationToken);

        if (!result.Success)
        {
            // Check if it's a not found error
            if (result.Message?.Contains("not found") == true)
            {
                return NotFound(result);
            }
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Search products by name or description with optional filtering
    /// </summary>
    /// <param name="name">Optional: Search term for name or description</param>
    /// <param name="franchiseId">Optional: Filter by franchise ID</param>
    /// <param name="productType">Optional: Filter by product type</param>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Items per page (default: 10)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated search results</returns>
    [HttpGet("search")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SearchProducts(
        [FromQuery] string? name = null,
        [FromQuery] int? franchiseId = null,
        [FromQuery] int? productType = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1 || pageSize < 1)
        {
            return BadRequest("Page number and page size must be greater than 0");
        }

        var result = await _productService.SearchProductsAsync(
            name,
            franchiseId,
            productType,
            pageNumber,
            pageSize,
            cancellationToken);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}
