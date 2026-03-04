using CyberpunkMarket.Models;
using cyberpunk_market_api.src.responses;

namespace cyberpunk_market_api.src.mappers;

public static class CartMapper
{
    public static CartItemResponse ToItemResponse(CartItem item)
    {
        return new CartItemResponse
        {
            Id = item.Id,
            ProductId = item.ProductId,
            ProductName = item.Product.Name,
            UnitPrice = item.Product.Price,
            Quantity = item.Quantity,
            Subtotal = item.Quantity * item.Product.Price
        };
    }

    public static CartResponse ToResponse(Cart cart)
    {
        var items = cart.Items.Select(ToItemResponse).ToList();
        var total = items.Sum(i => i.Subtotal);
        return new CartResponse
        {
            Id = cart.Id,
            UserId = cart.UserId,
            TotalAmount = total,
            Items = items
        };
    }
}
