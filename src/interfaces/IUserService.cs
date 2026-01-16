using System.Collections.Generic;
using System.Threading.Tasks;
using cyberpunk_market_api.src.dtos;
using cyberpunk_market_api.src.responses;

namespace cyberpunk_market_api.src.interfaces
{
    public interface IUserService
    {
        Task<List<UserResponse>> GetAllUsers();
        Task<UserResponse?> GetUserById(int id);
        Task<UserResponse?> GetUserByEmail(string email);
        Task<UserResponse> CreateUser(CreateUserDto dto);
        Task<UserResponse?> UpdateUser(int id, UpdateUserDto dto);
        Task<bool> DeleteUser(int id);
        Task<LoginResponse?> Login(LoginDto dto);
    }
}
