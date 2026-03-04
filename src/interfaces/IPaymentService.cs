using cyberpunk_market_api.src.responses;

namespace cyberpunk_market_api.src.interfaces;

public interface IPaymentService
{
    Task<ApiResponse<PaymentResponse?>> GetByOrderIdAsync(Guid userId, Guid orderId);
    Task<ApiResponse<PaymentResponse?>> MarkCompletedAsync(Guid userId, Guid orderId, string? externalId);
    Task<ApiResponse<PaymentResponse?>> MarkFailedAsync(Guid userId, Guid orderId, string? externalId);
}
