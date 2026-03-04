namespace CyberpunkMarket.Models;

public class Order : BaseEntity
{
    public Guid BuyerId { get; set; }
    public virtual User Buyer { get; set; } = null!;
    public Guid? ShippingAddressId { get; set; }
    public virtual Address? ShippingAddress { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public virtual ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    public virtual Payment? Payment { get; set; }
}