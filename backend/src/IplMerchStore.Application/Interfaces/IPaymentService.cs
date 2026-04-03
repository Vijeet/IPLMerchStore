using IplMerchStore.Application.Common;

namespace IplMerchStore.Application.Interfaces;

/// <summary>
/// Service interface for processing payments
/// </summary>
public interface IPaymentService
{
    /// <summary>
    /// Process payment for an order
    /// </summary>
    /// <param name="orderId">Order ID</param>
    /// <param name="amount">Payment amount</param>
    /// <param name="currency">Currency code (e.g., "INR")</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if payment successful, false otherwise</returns>
    Task<Result<bool>> ProcessPaymentAsync(int orderId, decimal amount, string currency = "INR", CancellationToken cancellationToken = default);
}
