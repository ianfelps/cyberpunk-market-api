using cyberpunk_market_api.src.dtos.Wishlist;
using cyberpunk_market_api.src.responses;

namespace cyberpunk_market_api.src.interfaces;

public interface IWishlistService
{
    Task<ApiResponse<IEnumerable<WishlistItemResponse>>> GetAllByUserAsync(Guid userId);
    Task<ApiResponse<WishlistItemResponse>> AddAsync(Guid userId, CreateWishlistItemDto dto);
    Task<ApiResponse<WishlistItemResponse?>> UpdateAsync(Guid userId, Guid id, UpdateWishlistItemDto dto);
    Task<ApiResponse<object>> DeleteAsync(Guid userId, Guid id);
}
