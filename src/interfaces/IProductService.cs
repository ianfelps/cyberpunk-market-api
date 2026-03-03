using cyberpunk_market_api.src.dtos;
using cyberpunk_market_api.src.responses;

namespace cyberpunk_market_api.src.interfaces;

public interface IProductService
{
    Task<ApiResponse<IEnumerable<ProductResponse>>> GetAllAsync();
    Task<ApiResponse<ProductResponse?>> GetByIdAsync(Guid id);
    Task<ApiResponse<ProductResponse>> CreateAsync(Guid userId, CreateProductDto dto);
    Task<ApiResponse<ProductResponse?>> UpdateAsync(Guid userId, Guid productId, UpdateProductDto dto);
    Task<ApiResponse<object>> DeleteAsync(Guid userId, Guid productId);
}
