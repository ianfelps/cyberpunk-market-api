using CyberpunkMarket.Models;
using cyberpunk_market_api.src.responses;

namespace cyberpunk_market_api.src.mappers;

public static class WishlistMapper
{
    public static WishlistItemResponse ToResponse(WishlistItem item)
    {
        return new WishlistItemResponse
        {
            Id = item.Id,
            ProductId = item.ProductId,
            ProductName = item.Product.Name,
            Price = item.Product.Price,
            NotifyOnPriceDrop = item.NotifyOnPriceDrop,
            CreatedAt = item.CreatedAt,
            UpdatedAt = item.UpdatedAt
        };
    }
}
