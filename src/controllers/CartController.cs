using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using cyberpunk_market_api.src.dtos.Cart;
using cyberpunk_market_api.src.interfaces;
using cyberpunk_market_api.src.responses;
using Swashbuckle.AspNetCore.Annotations;

namespace cyberpunk_market_api.src.controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Buyer,Seller")]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Obter carrinho atual", Description = "Retorna o carrinho atual do usuário autenticado.")]
    public async Task<ActionResult<ApiResponse<CartResponse>>> GetCurrent()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized(ApiResponse<CartResponse>.Fail("Usuário não autorizado."));
        var result = await _cartService.GetCurrentAsync(userId);
        return Ok(result);
    }

    [HttpPost("items")]
    [SwaggerOperation(Summary = "Adicionar item ao carrinho", Description = "Adiciona um produto ao carrinho do usuário autenticado.")]
    public async Task<ActionResult<ApiResponse<CartResponse>>> AddItem([FromBody] AddCartItemDto dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized(ApiResponse<CartResponse>.Fail("Usuário não autorizado."));
        var result = await _cartService.AddItemAsync(userId, dto);
        if (!result.success)
            return BadRequest(result);
        return Ok(result);
    }

    [HttpPut("items/{id:guid}")]
    [SwaggerOperation(Summary = "Atualizar item do carrinho", Description = "Atualiza a quantidade de um item do carrinho do usuário autenticado.")]
    public async Task<ActionResult<ApiResponse<CartResponse>>> UpdateItem(Guid id, [FromBody] UpdateCartItemDto dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized(ApiResponse<CartResponse>.Fail("Usuário não autorizado."));
        var result = await _cartService.UpdateItemAsync(userId, id, dto);
        if (!result.success)
            return BadRequest(result);
        return Ok(result);
    }

    [HttpDelete("items/{id:guid}")]
    [SwaggerOperation(Summary = "Remover item do carrinho", Description = "Remove um item do carrinho do usuário autenticado.")]
    public async Task<ActionResult<ApiResponse<object>>> RemoveItem(Guid id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized(ApiResponse<object>.Fail("Usuário não autorizado."));
        var result = await _cartService.RemoveItemAsync(userId, id);
        if (!result.success)
            return BadRequest(result);
        return Ok(result);
    }

    [HttpDelete]
    [SwaggerOperation(Summary = "Esvaziar carrinho", Description = "Remove todos os itens do carrinho do usuário autenticado.")]
    public async Task<ActionResult<ApiResponse<object>>> Clear()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized(ApiResponse<object>.Fail("Usuário não autorizado."));
        var result = await _cartService.ClearAsync(userId);
        if (!result.success)
            return BadRequest(result);
        return Ok(result);
    }
}
