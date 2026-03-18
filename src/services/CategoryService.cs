using cyberpunk_market_api.src.constants;
using cyberpunk_market_api.src.contexts;
using cyberpunk_market_api.src.dtos.Category;
using cyberpunk_market_api.src.interfaces;
using cyberpunk_market_api.src.mappers;
using cyberpunk_market_api.src.responses;
using Microsoft.EntityFrameworkCore;

namespace cyberpunk_market_api.src.services;

public class CategoryService : ICategoryService
{
    private readonly ApplicationDbContext _context;

    public CategoryService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<PagedResponse<CategoryResponse>>> GetAllAsync(int page, int pageSize, string? name)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = PaginationConstants.DefaultPageSize;
        if (pageSize > PaginationConstants.MaxPageSize) pageSize = PaginationConstants.MaxPageSize;

        var query = _context.Categories.AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(c => c.Name.Contains(name));

        var totalCount = await query.CountAsync();
        var categories = await query
            .OrderBy(c => c.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var items = categories.Select(CategoryMapper.ToResponse);
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var paged = new PagedResponse<CategoryResponse>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            TotalPages = totalPages
        };

        return ApiResponse<PagedResponse<CategoryResponse>>.Success(paged);
    }

    public async Task<ApiResponse<CategoryResponse?>> GetByIdAsync(Guid id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null)
            return ApiResponse<CategoryResponse?>.Fail("Categoria não encontrada.");

        return ApiResponse<CategoryResponse?>.Success(CategoryMapper.ToResponse(category));
    }

    public async Task<ApiResponse<CategoryResponse>> CreateAsync(CreateCategoryDto dto)
    {
        // Verificar se já existe categoria com mesmo slug
        var slugExists = await _context.Categories.AnyAsync(c => c.Slug == dto.Slug);
        if (slugExists)
            return ApiResponse<CategoryResponse>.Fail("Já existe uma categoria com este slug.");

        var category = CategoryMapper.ToEntity(dto);
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        return ApiResponse<CategoryResponse>.Success(CategoryMapper.ToResponse(category), "Categoria criada com sucesso.");
    }

    public async Task<ApiResponse<CategoryResponse?>> UpdateAsync(Guid id, UpdateCategoryDto dto)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null)
            return ApiResponse<CategoryResponse?>.Fail("Categoria não encontrada.");

        // Verificar se o novo slug já existe (e não é o da categoria atual)
        if (!string.IsNullOrWhiteSpace(dto.Slug) && dto.Slug != category.Slug)
        {
            var slugExists = await _context.Categories.AnyAsync(c => c.Slug == dto.Slug && c.Id != id);
            if (slugExists)
                return ApiResponse<CategoryResponse?>.Fail("Já existe uma categoria com este slug.");

            category.Slug = dto.Slug;
        }

        if (!string.IsNullOrWhiteSpace(dto.Name))
            category.Name = dto.Name;

        category.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return ApiResponse<CategoryResponse?>.Success(CategoryMapper.ToResponse(category), "Categoria atualizada com sucesso.");
    }

    public async Task<ApiResponse<object>> DeleteAsync(Guid id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null)
            return ApiResponse<object>.Fail("Categoria não encontrada.");

        // Verificar se há produtos vinculados
        var hasProducts = await _context.Products.AnyAsync(p => p.CategoryId == id);
        if (hasProducts)
            return ApiResponse<object>.Fail("Não é possível deletar uma categoria que possui produtos vinculados.");

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();

        return ApiResponse<object>.Success(new { }, "Categoria removida com sucesso.");
    }
}
