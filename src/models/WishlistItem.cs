namespace CyberpunkMarket.Models;

public class WishlistItem : BaseEntity
{
    public Guid UserId { get; set; }
    public virtual User User { get; set; } = null!;
    public Guid ProductId { get; set; }
    public virtual Product Product { get; set; } = null!;
    public bool NotifyOnPriceDrop { get; set; } = true;
}