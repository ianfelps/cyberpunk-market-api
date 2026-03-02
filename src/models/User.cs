namespace CyberpunkMarket.Models;

public class User : BaseEntity
{
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public UserRole Role { get; set; }
    public virtual Seller? SellerProfile { get; set; }
}
