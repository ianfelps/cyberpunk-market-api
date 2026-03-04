namespace cyberpunk_market_api.src.dtos.Wishlist;

public class CreateWishlistItemDto
{
    public Guid ProductId { get; set; }
    public bool NotifyOnPriceDrop { get; set; } = true;
}
