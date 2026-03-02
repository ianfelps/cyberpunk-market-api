namespace CyberpunkMarket.Models;

public class Category : BaseEntity
{
    public required string Name { get; set; }
    public required string Slug { get; set; }
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
