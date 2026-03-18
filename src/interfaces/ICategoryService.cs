using cyberpunk_market_api.src.dtos.Category;
using cyberpunk_market_api.src.responses;

namespace cyberpunk_market_api.src.interfaces;

public interface ICategoryService
{
    Task<ApiResponse<PagedResponse<CategoryResponse>>> GetAllAsync(int page, int pageSize, string? name);
    Task<ApiResponse<CategoryResponse?>> GetByIdAsync(Guid id);
    Task<ApiResponse<CategoryResponse>> CreateAsync(CreateCategoryDto dto);
    Task<ApiResponse<CategoryResponse?>> UpdateAsync(Guid id, UpdateCategoryDto dto);
    Task<ApiResponse<object>> DeleteAsync(Guid id);
}
