using cyberpunk_market_api.src.contexts;
using cyberpunk_market_api.src.dtos.Wishlist;
using cyberpunk_market_api.src.interfaces;
using cyberpunk_market_api.src.mappers;
using cyberpunk_market_api.src.responses;
using CyberpunkMarket.Models;
using Microsoft.EntityFrameworkCore;

namespace cyberpunk_market_api.src.services;

public class WishlistService : IWishlistService
{
    private readonly ApplicationDbContext _context;

    public WishlistService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<IEnumerable<WishlistItemResponse>>> GetAllByUserAsync(Guid userId)
    {
        var items = await _context.WishlistItems
            .Include(w => w.Product)
            .Where(w => w.UserId == userId)
            .OrderByDescending(w => w.CreatedAt)
            .ToListAsync();
        var responseItems = items.Select(WishlistMapper.ToResponse);
        return ApiResponse<IEnumerable<WishlistItemResponse>>.Success(responseItems);
    }

    public async Task<ApiResponse<WishlistItemResponse>> AddAsync(Guid userId, CreateWishlistItemDto dto)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == dto.ProductId && p.IsActive);
        if (product == null)
            return ApiResponse<WishlistItemResponse>.Fail("Produto não encontrado.");
        var exists = await _context.WishlistItems.AnyAsync(w => w.UserId == userId && w.ProductId == dto.ProductId);
        if (exists)
            return ApiResponse<WishlistItemResponse>.Fail("Produto já está na wishlist.");
        var entity = new WishlistItem
        {
            UserId = userId,
            ProductId = dto.ProductId,
            Product = product,
            NotifyOnPriceDrop = dto.NotifyOnPriceDrop
        };
        _context.WishlistItems.Add(entity);
        await _context.SaveChangesAsync();
        var reloaded = await _context.WishlistItems
            .Include(w => w.Product)
            .FirstAsync(w => w.Id == entity.Id);
        var response = WishlistMapper.ToResponse(reloaded);
        return ApiResponse<WishlistItemResponse>.Success(response, "Produto adicionado à wishlist.");
    }

    public async Task<ApiResponse<WishlistItemResponse?>> UpdateAsync(Guid userId, Guid id, UpdateWishlistItemDto dto)
    {
        var item = await _context.WishlistItems
            .Include(w => w.Product)
            .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);
        if (item == null)
            return ApiResponse<WishlistItemResponse?>.Fail("Item da wishlist não encontrado.");
        if (dto.NotifyOnPriceDrop.HasValue)
            item.NotifyOnPriceDrop = dto.NotifyOnPriceDrop.Value;
        item.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        var response = WishlistMapper.ToResponse(item);
        return ApiResponse<WishlistItemResponse?>.Success(response, "Wishlist atualizada com sucesso.");
    }

    public async Task<ApiResponse<object>> DeleteAsync(Guid userId, Guid id)
    {
        var item = await _context.WishlistItems.FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);
        if (item == null)
            return ApiResponse<object>.Fail("Item da wishlist não encontrado.");
        _context.WishlistItems.Remove(item);
        await _context.SaveChangesAsync();
        return ApiResponse<object>.Success(new { }, "Item removido da wishlist.");
    }
}
