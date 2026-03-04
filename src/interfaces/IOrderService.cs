using cyberpunk_market_api.src.dtos.Order;
using cyberpunk_market_api.src.responses;

namespace cyberpunk_market_api.src.interfaces;

public interface IOrderService
{
    Task<ApiResponse<PagedResponse<OrderResponse>>> GetMyOrdersAsync(Guid userId, int page, int pageSize, int? status);
    Task<ApiResponse<OrderResponse?>> GetByIdAsync(Guid userId, Guid id);
    Task<ApiResponse<OrderResponse>> CreateFromCartAsync(Guid userId, CreateOrderDto dto);
    Task<ApiResponse<OrderResponse?>> CancelAsync(Guid userId, Guid id);
}
