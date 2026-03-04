using cyberpunk_market_api.src.constants;
using cyberpunk_market_api.src.contexts;
using cyberpunk_market_api.src.dtos.Order;
using cyberpunk_market_api.src.interfaces;
using cyberpunk_market_api.src.mappers;
using cyberpunk_market_api.src.responses;
using CyberpunkMarket.Models;
using Microsoft.EntityFrameworkCore;

namespace cyberpunk_market_api.src.services;

public class OrderService : IOrderService
{
    private readonly ApplicationDbContext _context;

    public OrderService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<PagedResponse<OrderResponse>>> GetMyOrdersAsync(Guid userId, int page, int pageSize, int? status)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = PaginationConstants.DefaultPageSize;
        if (pageSize > PaginationConstants.MaxPageSize) pageSize = PaginationConstants.MaxPageSize;
        var query = _context.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .Include(o => o.Payment)
            .Where(o => o.BuyerId == userId);
        if (status.HasValue)
        {
            var statusEnum = (OrderStatus)status.Value;
            query = query.Where(o => o.Status == statusEnum);
        }
        var totalCount = await query.CountAsync();
        var orders = await query
            .OrderByDescending(o => o.OrderDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        var items = orders.Select(OrderMapper.ToResponse);
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        var paged = new PagedResponse<OrderResponse>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            TotalPages = totalPages
        };
        return ApiResponse<PagedResponse<OrderResponse>>.Success(paged);
    }

    public async Task<ApiResponse<OrderResponse?>> GetByIdAsync(Guid userId, Guid id)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .Include(o => o.Payment)
            .FirstOrDefaultAsync(o => o.Id == id && o.BuyerId == userId);
        if (order == null)
            return ApiResponse<OrderResponse?>.Fail("Pedido não encontrado.");
        var response = OrderMapper.ToResponse(order);
        return ApiResponse<OrderResponse?>.Success(response);
    }

    public async Task<ApiResponse<OrderResponse>> CreateFromCartAsync(Guid userId, CreateOrderDto dto)
    {
        var cart = await _context.Carts
            .Include(c => c.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId);
        if (cart == null || !cart.Items.Any())
            return ApiResponse<OrderResponse>.Fail("Carrinho vazio.");
        var address = await _context.Addresses.FirstOrDefaultAsync(a => a.Id == dto.ShippingAddressId && a.UserId == userId);
        if (address == null)
            return ApiResponse<OrderResponse>.Fail("Endereço de entrega não encontrado.");
        if (!Enum.IsDefined(typeof(PaymentMethod), dto.PaymentMethod))
            return ApiResponse<OrderResponse>.Fail("Método de pagamento inválido.");
        var method = (PaymentMethod)dto.PaymentMethod;
        var order = OrderMapper.ToEntityFromCart(dto, userId, dto.ShippingAddressId, cart, method);
        var payment = new Payment
        {
            Order = order,
            Amount = order.TotalAmount,
            Method = method,
            Status = PaymentStatus.Pending
        };
        order.Payment = payment;
        _context.Orders.Add(order);
        _context.Payments.Add(payment);
        var cartItems = cart.Items.ToList();
        _context.CartItems.RemoveRange(cartItems);
        cart.Items.Clear();
        await _context.SaveChangesAsync();
        var reloaded = await _context.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .Include(o => o.Payment)
            .FirstAsync(o => o.Id == order.Id);
        var response = OrderMapper.ToResponse(reloaded);
        return ApiResponse<OrderResponse>.Success(response, "Pedido criado com sucesso.");
    }

    public async Task<ApiResponse<OrderResponse?>> CancelAsync(Guid userId, Guid id)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .Include(o => o.Payment)
            .FirstOrDefaultAsync(o => o.Id == id && o.BuyerId == userId);
        if (order == null)
            return ApiResponse<OrderResponse?>.Fail("Pedido não encontrado.");
        if (order.Status != OrderStatus.Pending)
            return ApiResponse<OrderResponse?>.Fail("Somente pedidos pendentes podem ser cancelados.");
        order.Status = OrderStatus.Canceled;
        if (order.Payment != null && order.Payment.Status == PaymentStatus.Pending)
            order.Payment.Status = PaymentStatus.Failed;
        order.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        var response = OrderMapper.ToResponse(order);
        return ApiResponse<OrderResponse?>.Success(response, "Pedido cancelado com sucesso.");
    }
}
