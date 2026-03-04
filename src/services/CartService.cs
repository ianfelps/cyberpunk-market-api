using cyberpunk_market_api.src.contexts;
using cyberpunk_market_api.src.dtos.Cart;
using cyberpunk_market_api.src.interfaces;
using cyberpunk_market_api.src.mappers;
using cyberpunk_market_api.src.responses;
using CyberpunkMarket.Models;
using Microsoft.EntityFrameworkCore;

namespace cyberpunk_market_api.src.services;

public class CartService : ICartService
{
    private readonly ApplicationDbContext _context;

    public CartService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<CartResponse>> GetCurrentAsync(Guid userId)
    {
        var cart = await _context.Carts
            .Include(c => c.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId);
        if (cart == null)
        {
            cart = new Cart
            {
                UserId = userId
            };
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
            cart = await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstAsync(c => c.Id == cart.Id);
        }
        var response = CartMapper.ToResponse(cart);
        return ApiResponse<CartResponse>.Success(response);
    }

    public async Task<ApiResponse<CartResponse>> AddItemAsync(Guid userId, AddCartItemDto dto)
    {
        if (dto.Quantity <= 0)
            return ApiResponse<CartResponse>.Fail("Quantidade deve ser maior que zero.");
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == dto.ProductId && p.IsActive);
        if (product == null)
            return ApiResponse<CartResponse>.Fail("Produto não encontrado.");
        var cart = await _context.Carts
            .Include(c => c.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId);
        if (cart == null)
        {
            cart = new Cart
            {
                UserId = userId
            };
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
            cart = await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstAsync(c => c.Id == cart.Id);
        }
        var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == dto.ProductId);
        if (existingItem != null)
        {
            existingItem.Quantity += dto.Quantity;
        }
        else
        {
            var item = new CartItem
            {
                CartId = cart.Id,
                ProductId = dto.ProductId,
                Product = product,
                Quantity = dto.Quantity
            };
            cart.Items.Add(item);
        }
        await _context.SaveChangesAsync();
        var response = CartMapper.ToResponse(cart);
        return ApiResponse<CartResponse>.Success(response, "Item adicionado ao carrinho.");
    }

    public async Task<ApiResponse<CartResponse>> UpdateItemAsync(Guid userId, Guid cartItemId, UpdateCartItemDto dto)
    {
        if (dto.Quantity < 0)
            return ApiResponse<CartResponse>.Fail("Quantidade não pode ser negativa.");
        var cart = await _context.Carts
            .Include(c => c.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId);
        if (cart == null)
            return ApiResponse<CartResponse>.Fail("Carrinho não encontrado.");
        var item = cart.Items.FirstOrDefault(i => i.Id == cartItemId);
        if (item == null)
            return ApiResponse<CartResponse>.Fail("Item do carrinho não encontrado.");
        if (dto.Quantity == 0)
        {
            cart.Items.Remove(item);
            _context.CartItems.Remove(item);
        }
        else
        {
            item.Quantity = dto.Quantity;
        }
        await _context.SaveChangesAsync();
        var response = CartMapper.ToResponse(cart);
        return ApiResponse<CartResponse>.Success(response, "Carrinho atualizado com sucesso.");
    }

    public async Task<ApiResponse<object>> RemoveItemAsync(Guid userId, Guid cartItemId)
    {
        var cart = await _context.Carts
            .Include(c => c.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId);
        if (cart == null)
            return ApiResponse<object>.Fail("Carrinho não encontrado.");
        var item = cart.Items.FirstOrDefault(i => i.Id == cartItemId);
        if (item == null)
            return ApiResponse<object>.Fail("Item do carrinho não encontrado.");
        cart.Items.Remove(item);
        _context.CartItems.Remove(item);
        await _context.SaveChangesAsync();
        return ApiResponse<object>.Success(new { }, "Item removido do carrinho.");
    }

    public async Task<ApiResponse<object>> ClearAsync(Guid userId)
    {
        var cart = await _context.Carts
            .Include(c => c.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId);
        if (cart == null)
            return ApiResponse<object>.Fail("Carrinho não encontrado.");
        var items = cart.Items.ToList();
        if (items.Count == 0)
            return ApiResponse<object>.Success(new { }, "Carrinho já está vazio.");
        _context.CartItems.RemoveRange(items);
        cart.Items.Clear();
        await _context.SaveChangesAsync();
        return ApiResponse<object>.Success(new { }, "Carrinho esvaziado com sucesso.");
    }
}
