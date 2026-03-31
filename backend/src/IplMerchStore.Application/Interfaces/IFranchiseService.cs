using IplMerchStore.Application.Common;
using IplMerchStore.Application.DTOs;

namespace IplMerchStore.Application.Interfaces;

/// <summary>
/// Service interface for managing franchises
/// </summary>
public interface IFranchiseService
{
    Task<Result<IEnumerable<FranchiseDto>>> GetAllFranchisesAsync(CancellationToken cancellationToken = default);
    Task<Result<FranchiseDto?>> GetFranchiseByIdAsync(int id, CancellationToken cancellationToken = default);
}
