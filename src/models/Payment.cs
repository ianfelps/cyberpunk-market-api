namespace CyberpunkMarket.Models;

public class Payment : BaseEntity
{
    public Guid OrderId { get; set; }
    public virtual Order Order { get; set; } = null!;
    public decimal Amount { get; set; }
    public PaymentMethod Method { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public string? ExternalId { get; set; }
    public DateTime? PaidAt { get; set; }
}
