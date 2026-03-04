namespace cyberpunk_market_api.src.dtos.Cart;

public class AddCartItemDto
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}
