using cyberpunk_market_api.src.dtos.Cart;
using cyberpunk_market_api.src.responses;

namespace cyberpunk_market_api.src.interfaces;

public interface ICartService
{
    Task<ApiResponse<CartResponse>> GetCurrentAsync(Guid userId);
    Task<ApiResponse<CartResponse>> AddItemAsync(Guid userId, AddCartItemDto dto);
    Task<ApiResponse<CartResponse>> UpdateItemAsync(Guid userId, Guid cartItemId, UpdateCartItemDto dto);
    Task<ApiResponse<object>> RemoveItemAsync(Guid userId, Guid cartItemId);
    Task<ApiResponse<object>> ClearAsync(Guid userId);
}
