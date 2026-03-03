using CyberpunkMarket.Models;
using cyberpunk_market_api.src.dtos;
using cyberpunk_market_api.src.responses;

namespace cyberpunk_market_api.src.mappers;

public static class ProductMapper
{
    public static ProductResponse ToResponse(Product product)
    {
        return new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            IsActive = product.IsActive,
            SellerId = product.SellerId,
            CategoryId = product.CategoryId,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
    }

    public static Product ToEntity(CreateProductDto dto, Guid sellerId)
    {
        return new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            StockQuantity = dto.StockQuantity,
            SellerId = sellerId,
            CategoryId = dto.CategoryId
        };
    }
}
