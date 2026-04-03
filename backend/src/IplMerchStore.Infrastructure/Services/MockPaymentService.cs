using IplMerchStore.Application.Common;
using IplMerchStore.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace IplMerchStore.Infrastructure.Services;

/// <summary>
/// Mock payment service for testing and development
/// Always returns success (true) for any payment
/// 
/// Use Case:
/// - Development and testing without real payment gateway
/// - Demo purposes
/// - Placeholder for future real payment integration
/// </summary>
public class MockPaymentService : IPaymentService
{
    private readonly ILogger<MockPaymentService> _logger;

    public MockPaymentService(ILogger<MockPaymentService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Process payment - always returns success in mock implementation
    /// </summary>
    public async Task<Result<bool>> ProcessPaymentAsync(
        int orderId,
        decimal amount,
        string currency = "INR",
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Simulate async payment processing delay
            await Task.Delay(100, cancellationToken);

            _logger.LogInformation(
                "Mock payment processed: Order {OrderId}, Amount: {Amount} {Currency}",
                orderId,
                amount,
                currency);

            // Always return success
            return Result<bool>.SuccessResult(true, "Payment processed successfully (mock)");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing mock payment for order {OrderId}", orderId);
            return Result<bool>.FailureResult("An error occurred while processing payment");
        }
    }
}
