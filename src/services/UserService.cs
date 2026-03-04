using cyberpunk_market_api.src.constants;
using cyberpunk_market_api.src.contexts;
using cyberpunk_market_api.src.dtos.User;
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

    public async Task<ApiResponse<PagedResponse<UserResponse>>> GetAllAsync(int page, int pageSize, string? name, string? email, UserRole? role)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = PaginationConstants.DefaultPageSize;
        if (pageSize > PaginationConstants.MaxPageSize) pageSize = PaginationConstants.MaxPageSize;
        var query = _context.Users.AsQueryable();
        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(u => u.Name.Contains(name));
        if (!string.IsNullOrWhiteSpace(email))
            query = query.Where(u => u.Email.Contains(email));
        if (role.HasValue)
            query = query.Where(u => u.Role == role.Value);
        var totalCount = await query.CountAsync();
        var users = await query
            .OrderBy(u => u.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        var items = users.Select(UserMapper.ToResponse);
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        var paged = new PagedResponse<UserResponse>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            TotalPages = totalPages
        };
        return ApiResponse<PagedResponse<UserResponse>>.Success(paged);
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
        var seller = await _context.Sellers.FirstOrDefaultAsync(s => s.UserId == id);
        if (seller != null)
        {
            var hasProducts = await _context.Products.AnyAsync(p => p.SellerId == seller.Id);
            if (hasProducts)
                return ApiResponse<object>.Fail("Não é possível excluir usuário com perfil de vendedor que possui produtos.");
            _context.Sellers.Remove(seller);
        }
        var hasOrders = await _context.Orders.AnyAsync(o => o.BuyerId == id);
        if (hasOrders)
            return ApiResponse<object>.Fail("Não é possível excluir usuário com pedidos associados.");
        var hasAddresses = await _context.Addresses.AnyAsync(a => a.UserId == id);
        if (hasAddresses)
            return ApiResponse<object>.Fail("Não é possível excluir usuário com endereços cadastrados.");
        var hasCart = await _context.Carts.AnyAsync(c => c.UserId == id);
        if (hasCart)
            return ApiResponse<object>.Fail("Não é possível excluir usuário com carrinho ativo.");
        var hasReviews = await _context.Reviews.AnyAsync(r => r.UserId == id);
        if (hasReviews)
            return ApiResponse<object>.Fail("Não é possível excluir usuário com avaliações realizadas.");
        var hasWishlist = await _context.WishlistItems.AnyAsync(w => w.UserId == id);
        if (hasWishlist)
            return ApiResponse<object>.Fail("Não é possível excluir usuário com itens na lista de desejos.");
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
