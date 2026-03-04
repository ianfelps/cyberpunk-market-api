namespace CyberpunkMarket.Models;

public class Product : BaseEntity
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public bool IsActive { get; set; } = true;
    public Guid SellerId { get; set; }
    public virtual Seller Seller { get; set; } = null!;
    public Guid CategoryId { get; set; }
    public virtual Category Category { get; set; } = null!;
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
