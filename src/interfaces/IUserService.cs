using cyberpunk_market_api.src.dtos;
using cyberpunk_market_api.src.responses;

namespace cyberpunk_market_api.src.interfaces;

public interface IUserService
{
    Task<ApiResponse<UserResponse>> CreateBuyerAsync(CreateBuyerDto dto);
    Task<ApiResponse<UserResponse>> CreateSellerAsync(CreateSellerDto dto);
    Task<ApiResponse<UserResponse?>> GetByIdAsync(Guid id);
    Task<ApiResponse<IEnumerable<UserResponse>>> GetAllAsync();
    Task<ApiResponse<UserResponse?>> UpdateAsync(Guid id, UpdateUserDto dto);
    Task<ApiResponse<object>> DeleteAsync(Guid id);
    Task<ApiResponse<LoginResponse?>> LoginAsync(LoginDto dto);
}
