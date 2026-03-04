namespace CyberpunkMarket.Models;

public class User : BaseEntity
{
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public UserRole Role { get; set; }
    public virtual Seller? SellerProfile { get; set; }
    public virtual Cart? Cart { get; set; }
    public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    public virtual ICollection<WishlistItem> Wishlist { get; set; } = new List<WishlistItem>();
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
