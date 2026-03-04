using CyberpunkMarket.Models;
using cyberpunk_market_api.src.dtos.Order;
using cyberpunk_market_api.src.responses;

namespace cyberpunk_market_api.src.mappers;

public static class OrderMapper
{
    public static OrderResponse ToResponse(Order order)
    {
        var items = order.Items.Select(ToItemResponse).ToList();
        var payment = order.Payment != null ? ToPaymentResponse(order.Payment) : null;
        return new OrderResponse
        {
            Id = order.Id,
            BuyerId = order.BuyerId,
            ShippingAddressId = order.ShippingAddressId,
            OrderDate = order.OrderDate,
            TotalAmount = order.TotalAmount,
            Status = order.Status,
            Items = items,
            Payment = payment
        };
    }

    public static OrderItemResponse ToItemResponse(OrderItem item)
    {
        var subtotal = (item.UnitPrice - item.Discount) * item.Quantity;
        return new OrderItemResponse
        {
            Id = item.Id,
            ProductId = item.ProductId,
            ProductName = item.Product.Name,
            Quantity = item.Quantity,
            UnitPrice = item.UnitPrice,
            Discount = item.Discount,
            Subtotal = subtotal
        };
    }

    public static PaymentResponse ToPaymentResponse(Payment payment)
    {
        return new PaymentResponse
        {
            Id = payment.Id,
            Amount = payment.Amount,
            Method = payment.Method,
            Status = payment.Status,
            ExternalId = payment.ExternalId,
            PaidAt = payment.PaidAt
        };
    }

    public static Order ToEntityFromCart(CreateOrderDto dto, Guid userId, Guid shippingAddressId, Cart cart, PaymentMethod method)
    {
        var order = new Order
        {
            BuyerId = userId,
            ShippingAddressId = shippingAddressId,
            OrderDate = DateTime.UtcNow,
            Status = OrderStatus.Pending
        };
        var items = new List<OrderItem>();
        foreach (var cartItem in cart.Items)
        {
            var item = new OrderItem
            {
                Order = order,
                ProductId = cartItem.ProductId,
                Product = cartItem.Product,
                Quantity = cartItem.Quantity,
                UnitPrice = cartItem.Product.Price,
                Discount = 0
            };
            items.Add(item);
        }
        order.Items = items;
        order.TotalAmount = items.Sum(i => (i.UnitPrice - i.Discount) * i.Quantity);
        return order;
    }
}
