using IplMerchStore.Application.Common;
using IplMerchStore.Application.DTOs;

namespace IplMerchStore.Application.Interfaces;

/// <summary>
/// Service interface for managing franchises
/// </summary>
public interface IFranchiseService
{
    Task<Result<PagedResult<FranchiseDto>>> GetAllFranchisesAsync(int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default);
    Task<Result<FranchiseDto?>> GetFranchiseByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Result<FranchiseDto>> CreateFranchiseAsync(FranchiseInputDto inputDto, CancellationToken cancellationToken = default);
    Task<Result<FranchiseDto>> UpdateFranchiseAsync(int id, FranchiseInputDto inputDto, CancellationToken cancellationToken = default);
    Task<Result> DeleteFranchiseAsync(int id, CancellationToken cancellationToken = default);
}
