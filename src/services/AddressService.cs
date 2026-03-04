using cyberpunk_market_api.src.constants;
using cyberpunk_market_api.src.contexts;
using cyberpunk_market_api.src.dtos.Address;
using cyberpunk_market_api.src.interfaces;
using cyberpunk_market_api.src.mappers;
using cyberpunk_market_api.src.responses;
using CyberpunkMarket.Models;
using Microsoft.EntityFrameworkCore;

namespace cyberpunk_market_api.src.services;

public class AddressService : IAddressService
{
    private readonly ApplicationDbContext _context;

    public AddressService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<PagedResponse<AddressResponse>>> GetAllByUserAsync(Guid userId, int page, int pageSize, string? city, string? zipCode)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = PaginationConstants.DefaultPageSize;
        if (pageSize > PaginationConstants.MaxPageSize) pageSize = PaginationConstants.MaxPageSize;
        var query = _context.Addresses.Where(a => a.UserId == userId);
        if (!string.IsNullOrWhiteSpace(city))
            query = query.Where(a => a.City.Contains(city));
        if (!string.IsNullOrWhiteSpace(zipCode))
            query = query.Where(a => a.ZipCode.Contains(zipCode));
        var totalCount = await query.CountAsync();
        var addresses = await query
            .OrderBy(a => a.City).ThenBy(a => a.Street)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        var items = addresses.Select(AddressMapper.ToResponse);
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        var paged = new PagedResponse<AddressResponse>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            TotalPages = totalPages
        };
        return ApiResponse<PagedResponse<AddressResponse>>.Success(paged);
    }

    public async Task<ApiResponse<AddressResponse?>> GetByIdAsync(Guid userId, Guid id)
    {
        var address = await _context.Addresses.FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);
        if (address == null)
            return ApiResponse<AddressResponse?>.Fail("Endereço não encontrado.");
        return ApiResponse<AddressResponse?>.Success(AddressMapper.ToResponse(address));
    }

    public async Task<ApiResponse<AddressResponse>> CreateAsync(Guid userId, CreateAddressDto dto)
    {
        if (dto.IsDefault)
        {
            var others = await _context.Addresses.Where(a => a.UserId == userId).ToListAsync();
            foreach (var a in others)
                a.IsDefault = false;
        }
        var address = AddressMapper.ToEntity(dto, userId);
        _context.Addresses.Add(address);
        await _context.SaveChangesAsync();
        return ApiResponse<AddressResponse>.Success(AddressMapper.ToResponse(address), "Endereço cadastrado com sucesso.");
    }

    public async Task<ApiResponse<AddressResponse?>> UpdateAsync(Guid userId, Guid id, UpdateAddressDto dto)
    {
        var address = await _context.Addresses.FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);
        if (address == null)
            return ApiResponse<AddressResponse?>.Fail("Endereço não encontrado.");
        if (!string.IsNullOrWhiteSpace(dto.Street))
            address.Street = dto.Street;
        if (!string.IsNullOrWhiteSpace(dto.Number))
            address.Number = dto.Number;
        if (dto.Complement != null)
            address.Complement = dto.Complement;
        if (!string.IsNullOrWhiteSpace(dto.Neighborhood))
            address.Neighborhood = dto.Neighborhood;
        if (!string.IsNullOrWhiteSpace(dto.City))
            address.City = dto.City;
        if (!string.IsNullOrWhiteSpace(dto.State))
            address.State = dto.State;
        if (!string.IsNullOrWhiteSpace(dto.ZipCode))
            address.ZipCode = dto.ZipCode;
        if (dto.IsDefault.HasValue)
        {
            if (dto.IsDefault.Value)
            {
                var others = await _context.Addresses.Where(a => a.UserId == userId && a.Id != id).ToListAsync();
                foreach (var a in others)
                    a.IsDefault = false;
            }
            address.IsDefault = dto.IsDefault.Value;
        }
        address.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return ApiResponse<AddressResponse?>.Success(AddressMapper.ToResponse(address), "Endereço atualizado com sucesso.");
    }

    public async Task<ApiResponse<object>> DeleteAsync(Guid userId, Guid id)
    {
        var address = await _context.Addresses.FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);
        if (address == null)
            return ApiResponse<object>.Fail("Endereço não encontrado.");
        var usedInOrder = await _context.Orders.AnyAsync(o => o.ShippingAddressId == id);
        if (usedInOrder)
            return ApiResponse<object>.Fail("Não é possível excluir endereço utilizado em pedidos.");
        _context.Addresses.Remove(address);
        await _context.SaveChangesAsync();
        return ApiResponse<object>.Success(new { }, "Endereço removido com sucesso.");
    }
}
