using cyberpunk_market_api.src.dtos.Address;
using cyberpunk_market_api.src.responses;

namespace cyberpunk_market_api.src.interfaces;

public interface IAddressService
{
    Task<ApiResponse<PagedResponse<AddressResponse>>> GetAllByUserAsync(Guid userId, int page, int pageSize, string? city, string? zipCode);
    Task<ApiResponse<AddressResponse?>> GetByIdAsync(Guid userId, Guid id);
    Task<ApiResponse<AddressResponse>> CreateAsync(Guid userId, CreateAddressDto dto);
    Task<ApiResponse<AddressResponse?>> UpdateAsync(Guid userId, Guid id, UpdateAddressDto dto);
    Task<ApiResponse<object>> DeleteAsync(Guid userId, Guid id);
}
