using cyberpunk_market_api.src.contexts;
using cyberpunk_market_api.src.interfaces;
using cyberpunk_market_api.src.mappers;
using cyberpunk_market_api.src.responses;
using CyberpunkMarket.Models;
using Microsoft.EntityFrameworkCore;

namespace cyberpunk_market_api.src.services;

public class PaymentService : IPaymentService
{
    private readonly ApplicationDbContext _context;

    public PaymentService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<PaymentResponse?>> GetByOrderIdAsync(Guid userId, Guid orderId)
    {
        var order = await _context.Orders
            .Include(o => o.Payment)
            .FirstOrDefaultAsync(o => o.Id == orderId && o.BuyerId == userId);
        if (order == null || order.Payment == null)
            return ApiResponse<PaymentResponse?>.Fail("Pagamento não encontrado.");
        var response = OrderMapper.ToPaymentResponse(order.Payment);
        return ApiResponse<PaymentResponse?>.Success(response);
    }

    public async Task<ApiResponse<PaymentResponse?>> MarkCompletedAsync(Guid userId, Guid orderId, string? externalId)
    {
        var order = await _context.Orders
            .Include(o => o.Payment)
            .FirstOrDefaultAsync(o => o.Id == orderId && o.BuyerId == userId);
        if (order == null || order.Payment == null)
            return ApiResponse<PaymentResponse?>.Fail("Pagamento não encontrado.");
        if (order.Payment.Status == PaymentStatus.Completed)
            return ApiResponse<PaymentResponse?>.Fail("Pagamento já está concluído.");
        order.Payment.Status = PaymentStatus.Completed;
        order.Payment.ExternalId = externalId ?? order.Payment.ExternalId;
        order.Payment.PaidAt = DateTime.UtcNow;
        if (order.Status == OrderStatus.Pending)
            order.Status = OrderStatus.Paid;
        order.Payment.UpdatedAt = DateTime.UtcNow;
        order.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        var response = OrderMapper.ToPaymentResponse(order.Payment);
        return ApiResponse<PaymentResponse?>.Success(response, "Pagamento concluído com sucesso.");
    }

    public async Task<ApiResponse<PaymentResponse?>> MarkFailedAsync(Guid userId, Guid orderId, string? externalId)
    {
        var order = await _context.Orders
            .Include(o => o.Payment)
            .FirstOrDefaultAsync(o => o.Id == orderId && o.BuyerId == userId);
        if (order == null || order.Payment == null)
            return ApiResponse<PaymentResponse?>.Fail("Pagamento não encontrado.");
        if (order.Payment.Status == PaymentStatus.Failed)
            return ApiResponse<PaymentResponse?>.Fail("Pagamento já está marcado como falho.");
        order.Payment.Status = PaymentStatus.Failed;
        order.Payment.ExternalId = externalId ?? order.Payment.ExternalId;
        order.Payment.UpdatedAt = DateTime.UtcNow;
        if (order.Status == OrderStatus.Pending)
            order.Status = OrderStatus.Canceled;
        order.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        var response = OrderMapper.ToPaymentResponse(order.Payment);
        return ApiResponse<PaymentResponse?>.Success(response, "Pagamento marcado como falho.");
    }
}
