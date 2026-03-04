namespace cyberpunk_market_api.src.dtos.Order;

public class CreateOrderDto
{
    public Guid ShippingAddressId { get; set; }
    public int PaymentMethod { get; set; }
}
