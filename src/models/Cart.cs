namespace CyberpunkMarket.Models;

public class Cart : BaseEntity
{
    public Guid UserId { get; set; }
    public virtual User User { get; set; } = null!;
    public virtual ICollection<CartItem> Items { get; set; } = new List<CartItem>();
    public decimal TotalAmount => Items.Sum(item => item.Quantity * item.Product.Price);
}