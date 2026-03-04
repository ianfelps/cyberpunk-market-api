using cyberpunk_market_api.src.dtos.Review;
using cyberpunk_market_api.src.responses;

namespace cyberpunk_market_api.src.interfaces;

public interface IReviewService
{
    Task<ApiResponse<PagedResponse<ReviewResponse>>> GetByProductIdAsync(Guid productId, int page, int pageSize, int? minRating, int? maxRating);
    Task<ApiResponse<ReviewResponse?>> GetByIdAsync(Guid id);
    Task<ApiResponse<ReviewResponse>> CreateAsync(Guid userId, CreateReviewDto dto);
    Task<ApiResponse<ReviewResponse?>> UpdateAsync(Guid userId, Guid id, UpdateReviewDto dto);
    Task<ApiResponse<object>> DeleteAsync(Guid userId, Guid id);
}
