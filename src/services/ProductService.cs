using cyberpunk_market_api.src.contexts;
using cyberpunk_market_api.src.dtos;
using cyberpunk_market_api.src.interfaces;
using cyberpunk_market_api.src.mappers;
using cyberpunk_market_api.src.responses;
using Microsoft.EntityFrameworkCore;

namespace cyberpunk_market_api.src.services;

public class ProductService : IProductService
{
    private readonly ApplicationDbContext _context;

    public ProductService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<IEnumerable<ProductResponse>>> GetAllAsync()
    {
        var products = await _context.Products.ToListAsync();
        var response = products.Select(ProductMapper.ToResponse);
        return ApiResponse<IEnumerable<ProductResponse>>.Success(response);
    }

    public async Task<ApiResponse<ProductResponse?>> GetByIdAsync(Guid id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
            return ApiResponse<ProductResponse?>.Fail("Produto não encontrado.");
        return ApiResponse<ProductResponse?>.Success(ProductMapper.ToResponse(product));
    }

    public async Task<ApiResponse<ProductResponse>> CreateAsync(Guid userId, CreateProductDto dto)
    {
        var seller = await _context.Sellers.FirstOrDefaultAsync(s => s.UserId == userId);
        if (seller == null)
            return ApiResponse<ProductResponse>.Fail("Vendedor não encontrado.");
        var categoryExists = await _context.Categories.AnyAsync(c => c.Id == dto.CategoryId);
        if (!categoryExists)
            return ApiResponse<ProductResponse>.Fail("Categoria não encontrada.");
        var product = ProductMapper.ToEntity(dto, seller.Id);
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return ApiResponse<ProductResponse>.Success(ProductMapper.ToResponse(product), "Produto criado com sucesso.");
    }

    public async Task<ApiResponse<ProductResponse?>> UpdateAsync(Guid userId, Guid productId, UpdateProductDto dto)
    {
        var seller = await _context.Sellers.FirstOrDefaultAsync(s => s.UserId == userId);
        if (seller == null)
            return ApiResponse<ProductResponse?>.Fail("Vendedor não encontrado.");
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId && p.SellerId == seller.Id);
        if (product == null)
            return ApiResponse<ProductResponse?>.Fail("Produto não encontrado.");
        if (!string.IsNullOrWhiteSpace(dto.Name))
            product.Name = dto.Name;
        if (dto.Description != null)
            product.Description = dto.Description;
        if (dto.Price.HasValue)
            product.Price = dto.Price.Value;
        if (dto.StockQuantity.HasValue)
            product.StockQuantity = dto.StockQuantity.Value;
        if (dto.IsActive.HasValue)
            product.IsActive = dto.IsActive.Value;
        if (dto.CategoryId.HasValue)
        {
            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == dto.CategoryId.Value);
            if (!categoryExists)
                return ApiResponse<ProductResponse?>.Fail("Categoria não encontrada.");
            product.CategoryId = dto.CategoryId.Value;
        }
        product.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return ApiResponse<ProductResponse?>.Success(ProductMapper.ToResponse(product), "Produto atualizado com sucesso.");
    }

    public async Task<ApiResponse<object>> DeleteAsync(Guid userId, Guid productId)
    {
        var seller = await _context.Sellers.FirstOrDefaultAsync(s => s.UserId == userId);
        if (seller == null)
            return ApiResponse<object>.Fail("Vendedor não encontrado.");
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId && p.SellerId == seller.Id);
        if (product == null)
            return ApiResponse<object>.Fail("Produto não encontrado.");
        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return ApiResponse<object>.Success(new { }, "Produto removido com sucesso.");
    }
}
