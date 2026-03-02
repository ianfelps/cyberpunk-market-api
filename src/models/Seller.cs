namespace CyberpunkMarket.Models;

public class Seller : BaseEntity
{
    public Guid UserId { get; set; }
    public virtual User User { get; set; } = null!;
    public required string StoreName { get; set; }
    public string? Bio { get; set; }
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
