namespace CyberpunkMarket.Models;

public class Review : BaseEntity
{
    public Guid UserId { get; set; }
    public virtual User User { get; set; } = null!;
    public Guid ProductId { get; set; }
    public virtual Product Product { get; set; } = null!;
    public int Rating { get; set; }
    public string? Comment { get; set; }
}
