using cyberpunk_market_api.src.constants;
using cyberpunk_market_api.src.contexts;
using cyberpunk_market_api.src.dtos.Product;
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

    public async Task<ApiResponse<PagedResponse<ProductResponse>>> GetAllAsync(int page, int pageSize, string? name, Guid? categoryId, decimal? minPrice, decimal? maxPrice, bool? isActive)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = PaginationConstants.DefaultPageSize;
        if (pageSize > PaginationConstants.MaxPageSize) pageSize = PaginationConstants.MaxPageSize;
        var query = _context.Products.AsQueryable();
        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(p => p.Name.Contains(name));
        if (categoryId.HasValue)
            query = query.Where(p => p.CategoryId == categoryId.Value);
        if (minPrice.HasValue)
            query = query.Where(p => p.Price >= minPrice.Value);
        if (maxPrice.HasValue)
            query = query.Where(p => p.Price <= maxPrice.Value);
        if (isActive.HasValue)
            query = query.Where(p => p.IsActive == isActive.Value);
        var totalCount = await query.CountAsync();
        var products = await query
            .OrderBy(p => p.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        var items = products.Select(ProductMapper.ToResponse);
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        var paged = new PagedResponse<ProductResponse>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            TotalPages = totalPages
        };
        return ApiResponse<PagedResponse<ProductResponse>>.Success(paged);
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
