using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using cyberpunk_market_api.src.dtos.Wishlist;
using cyberpunk_market_api.src.interfaces;
using cyberpunk_market_api.src.responses;
using Swashbuckle.AspNetCore.Annotations;

namespace cyberpunk_market_api.src.controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Buyer,Seller")]
public class WishlistController : ControllerBase
{
    private readonly IWishlistService _wishlistService;

    public WishlistController(IWishlistService wishlistService)
    {
        _wishlistService = wishlistService;
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Listar wishlist", Description = "Retorna a lista de itens da wishlist do usuário autenticado.")]
    public async Task<ActionResult<ApiResponse<IEnumerable<WishlistItemResponse>>>> GetAll()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized(ApiResponse<IEnumerable<WishlistItemResponse>>.Fail("Usuário não autorizado."));
        var result = await _wishlistService.GetAllByUserAsync(userId);
        return Ok(result);
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Adicionar à wishlist", Description = "Adiciona um produto à wishlist do usuário autenticado.")]
    public async Task<ActionResult<ApiResponse<WishlistItemResponse>>> Add([FromBody] CreateWishlistItemDto dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized(ApiResponse<WishlistItemResponse>.Fail("Usuário não autorizado."));
        var result = await _wishlistService.AddAsync(userId, dto);
        if (!result.success)
            return BadRequest(result);
        return StatusCode(201, result);
    }

    [HttpPut("{id:guid}")]
    [SwaggerOperation(Summary = "Atualizar item da wishlist", Description = "Atualiza configurações de um item da wishlist do usuário autenticado.")]
    public async Task<ActionResult<ApiResponse<WishlistItemResponse?>>> Update(Guid id, [FromBody] UpdateWishlistItemDto dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized(ApiResponse<WishlistItemResponse?>.Fail("Usuário não autorizado."));
        var result = await _wishlistService.UpdateAsync(userId, id, dto);
        if (!result.success)
            return BadRequest(result);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [SwaggerOperation(Summary = "Remover item da wishlist", Description = "Remove um item da wishlist do usuário autenticado.")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized(ApiResponse<object>.Fail("Usuário não autorizado."));
        var result = await _wishlistService.DeleteAsync(userId, id);
        if (!result.success)
            return BadRequest(result);
        return Ok(result);
    }
}
