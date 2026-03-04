using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using cyberpunk_market_api.src.constants;
using cyberpunk_market_api.src.dtos.Order;
using cyberpunk_market_api.src.interfaces;
using cyberpunk_market_api.src.responses;
using Swashbuckle.AspNetCore.Annotations;

namespace cyberpunk_market_api.src.controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Buyer,Seller")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Listar pedidos do usuário", Description = "Retorna a lista paginada de pedidos do usuário autenticado. Query: page, pageSize, status.")]
    public async Task<ActionResult<ApiResponse<PagedResponse<OrderResponse>>>> GetMyOrders(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = PaginationConstants.DefaultPageSize,
        [FromQuery] int? status = null)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized(ApiResponse<PagedResponse<OrderResponse>>.Fail("Usuário não autorizado."));
        var result = await _orderService.GetMyOrdersAsync(userId, page, pageSize, status);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [SwaggerOperation(Summary = "Buscar pedido por ID", Description = "Busca um pedido do usuário autenticado pelo identificador.")]
    public async Task<ActionResult<ApiResponse<OrderResponse?>>> GetById(Guid id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized(ApiResponse<OrderResponse?>.Fail("Usuário não autorizado."));
        var result = await _orderService.GetByIdAsync(userId, id);
        if (!result.success)
            return NotFound(result);
        return Ok(result);
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Criar pedido a partir do carrinho", Description = "Cria um pedido usando os itens do carrinho do usuário autenticado.")]
    public async Task<ActionResult<ApiResponse<OrderResponse>>> CreateFromCart([FromBody] CreateOrderDto dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized(ApiResponse<OrderResponse>.Fail("Usuário não autorizado."));
        var result = await _orderService.CreateFromCartAsync(userId, dto);
        if (!result.success)
            return BadRequest(result);
        return StatusCode(201, result);
    }

    [HttpPost("{id:guid}/cancel")]
    [SwaggerOperation(Summary = "Cancelar pedido", Description = "Cancela um pedido pendente do usuário autenticado.")]
    public async Task<ActionResult<ApiResponse<OrderResponse?>>> Cancel(Guid id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized(ApiResponse<OrderResponse?>.Fail("Usuário não autorizado."));
        var result = await _orderService.CancelAsync(userId, id);
        if (!result.success)
            return BadRequest(result);
        return Ok(result);
    }
}
