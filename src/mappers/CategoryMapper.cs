using CyberpunkMarket.Models;
using cyberpunk_market_api.src.dtos.Category;
using cyberpunk_market_api.src.responses;

namespace cyberpunk_market_api.src.mappers;

public static class CategoryMapper
{
    public static CategoryResponse ToResponse(Category category)
    {
        return new CategoryResponse
        {
            Id = category.Id,
            Name = category.Name,
            Slug = category.Slug,
            CreatedAt = category.CreatedAt,
            UpdatedAt = category.UpdatedAt
        };
    }

    public static Category ToEntity(CreateCategoryDto dto)
    {
        return new Category
        {
            Name = dto.Name,
            Slug = dto.Slug
        };
    }
}
