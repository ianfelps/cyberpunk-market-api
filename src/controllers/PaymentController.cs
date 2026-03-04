using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using cyberpunk_market_api.src.interfaces;
using cyberpunk_market_api.src.responses;
using Swashbuckle.AspNetCore.Annotations;

namespace cyberpunk_market_api.src.controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Buyer,Seller")]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpGet("{orderId:guid}")]
    [SwaggerOperation(Summary = "Buscar pagamento por pedido", Description = "Retorna as informações de pagamento de um pedido do usuário autenticado.")]
    public async Task<ActionResult<ApiResponse<PaymentResponse?>>> GetByOrderId(Guid orderId)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized(ApiResponse<PaymentResponse?>.Fail("Usuário não autorizado."));
        var result = await _paymentService.GetByOrderIdAsync(userId, orderId);
        if (!result.success)
            return NotFound(result);
        return Ok(result);
    }

    [HttpPost("{orderId:guid}/complete")]
    [SwaggerOperation(Summary = "Concluir pagamento", Description = "Marca o pagamento de um pedido do usuário autenticado como concluído.")]
    public async Task<ActionResult<ApiResponse<PaymentResponse?>>> MarkCompleted(Guid orderId, [FromQuery] string? externalId = null)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized(ApiResponse<PaymentResponse?>.Fail("Usuário não autorizado."));
        var result = await _paymentService.MarkCompletedAsync(userId, orderId, externalId);
        if (!result.success)
            return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("{orderId:guid}/fail")]
    [SwaggerOperation(Summary = "Falha no pagamento", Description = "Marca o pagamento de um pedido do usuário autenticado como falho.")]
    public async Task<ActionResult<ApiResponse<PaymentResponse?>>> MarkFailed(Guid orderId, [FromQuery] string? externalId = null)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized(ApiResponse<PaymentResponse?>.Fail("Usuário não autorizado."));
        var result = await _paymentService.MarkFailedAsync(userId, orderId, externalId);
        if (!result.success)
            return BadRequest(result);
        return Ok(result);
    }
}
