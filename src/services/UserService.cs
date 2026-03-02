using cyberpunk_market_api.src.contexts;
using cyberpunk_market_api.src.dtos;
using cyberpunk_market_api.src.interfaces;
using cyberpunk_market_api.src.mappers;
using cyberpunk_market_api.src.responses;
using CyberpunkMarket.Models;
using Microsoft.EntityFrameworkCore;

namespace cyberpunk_market_api.src.services;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;
    private readonly JwtService _jwtService;

    public UserService(ApplicationDbContext context, JwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    public async Task<ApiResponse<UserResponse>> CreateBuyerAsync(CreateBuyerDto dto)
    {
        var exists = await _context.Users.AnyAsync(u => u.Email == dto.Email);
        if (exists)
            return ApiResponse<UserResponse>.Fail("E-mail já cadastrado.");
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        var user = UserMapper.ToEntity(dto, passwordHash);
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return ApiResponse<UserResponse>.Success(UserMapper.ToResponse(user), "Comprador cadastrado com sucesso.");
    }

    public async Task<ApiResponse<UserResponse>> CreateSellerAsync(CreateSellerDto dto)
    {
        var exists = await _context.Users.AnyAsync(u => u.Email == dto.Email);
        if (exists)
            return ApiResponse<UserResponse>.Fail("E-mail já cadastrado.");
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        var user = UserMapper.ToEntity(dto, passwordHash);
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        var seller = new Seller
        {
            UserId = user.Id,
            StoreName = dto.StoreName,
            Bio = dto.Bio
        };
        _context.Sellers.Add(seller);
        await _context.SaveChangesAsync();
        return ApiResponse<UserResponse>.Success(UserMapper.ToResponse(user), "Vendedor cadastrado com sucesso.");
    }

    public async Task<ApiResponse<UserResponse?>> GetByIdAsync(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return ApiResponse<UserResponse?>.Fail("Usuário não encontrado.");
        return ApiResponse<UserResponse?>.Success(UserMapper.ToResponse(user));
    }

    public async Task<ApiResponse<IEnumerable<UserResponse>>> GetAllAsync()
    {
        var users = await _context.Users.ToListAsync();
        var response = users.Select(UserMapper.ToResponse);
        return ApiResponse<IEnumerable<UserResponse>>.Success(response);
    }

    public async Task<ApiResponse<UserResponse?>> UpdateAsync(Guid id, UpdateUserDto dto)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return ApiResponse<UserResponse?>.Fail("Usuário não encontrado.");
        if (!string.IsNullOrWhiteSpace(dto.Name))
            user.Name = dto.Name;
        if (!string.IsNullOrWhiteSpace(dto.Email))
        {
            var exists = await _context.Users.AnyAsync(u => u.Email == dto.Email && u.Id != id);
            if (exists)
                return ApiResponse<UserResponse?>.Fail("E-mail já está em uso.");
            user.Email = dto.Email;
        }
        if (!string.IsNullOrWhiteSpace(dto.Password))
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        if (dto.Role.HasValue)
            user.Role = dto.Role.Value;
        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return ApiResponse<UserResponse?>.Success(UserMapper.ToResponse(user), "Usuário atualizado com sucesso.");
    }

    public async Task<ApiResponse<object>> DeleteAsync(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return ApiResponse<object>.Fail("Usuário não encontrado.");
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return ApiResponse<object>.Success(new { }, "Usuário removido com sucesso.");
    }

    public async Task<ApiResponse<LoginResponse?>> LoginAsync(LoginDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return ApiResponse<LoginResponse?>.Fail("E-mail ou senha inválidos.");
        var token = _jwtService.GenerateToken(user);
        var response = new LoginResponse
        {
            Token = token,
            User = UserMapper.ToResponse(user)
        };
        return ApiResponse<LoginResponse?>.Success(response, "Login realizado com sucesso.");
    }
}
