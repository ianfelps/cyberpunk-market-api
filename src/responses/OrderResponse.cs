using CyberpunkMarket.Models;

namespace cyberpunk_market_api.src.responses;

public class OrderItemResponse
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; }
    public decimal Subtotal { get; set; }
}

public class PaymentResponse
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod Method { get; set; }
    public PaymentStatus Status { get; set; }
    public string? ExternalId { get; set; }
    public DateTime? PaidAt { get; set; }
}

public class OrderResponse
{
    public Guid Id { get; set; }
    public Guid BuyerId { get; set; }
    public Guid? ShippingAddressId { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; }
    public IEnumerable<OrderItemResponse> Items { get; set; } = new List<OrderItemResponse>();
    public PaymentResponse? Payment { get; set; }
}
