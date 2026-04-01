using IplMerchStore.Application.DTOs;
using IplMerchStore.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IplMerchStore.Api.Controllers;

/// <summary>
/// Controller for managing franchises
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class FranchisesController : ControllerBase
{
    private readonly IFranchiseService _franchiseService;

    public FranchisesController(IFranchiseService franchiseService)
    {
        _franchiseService = franchiseService;
    }

    /// <summary>
    /// Get all franchises with pagination
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetFranchises([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1 || pageSize < 1)
        {
            return BadRequest("Page number and page size must be greater than 0");
        }

        var result = await _franchiseService.GetAllFranchisesAsync(pageNumber, pageSize, cancellationToken);
        
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Get a specific franchise by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetFranchiseById(int id, CancellationToken cancellationToken = default)
    {
        var result = await _franchiseService.GetFranchiseByIdAsync(id, cancellationToken);
        
        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Create a new franchise
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateFranchise(FranchiseInputDto inputDto, CancellationToken cancellationToken = default)
    {
        var result = await _franchiseService.CreateFranchiseAsync(inputDto, cancellationToken);
        
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return CreatedAtAction(nameof(GetFranchiseById), new { id = result.Data?.Id }, result);
    }

    /// <summary>
    /// Update an existing franchise
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateFranchise(int id, FranchiseInputDto inputDto, CancellationToken cancellationToken = default)
    {
        var result = await _franchiseService.UpdateFranchiseAsync(id, inputDto, cancellationToken);
        
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
    /// Delete a franchise
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteFranchise(int id, CancellationToken cancellationToken = default)
    {
        var result = await _franchiseService.DeleteFranchiseAsync(id, cancellationToken);
        
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
}
