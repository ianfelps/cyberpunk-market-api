using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using cyberpunk_market_api.src.constants;
using cyberpunk_market_api.src.dtos.Review;
using cyberpunk_market_api.src.interfaces;
using cyberpunk_market_api.src.responses;
using Swashbuckle.AspNetCore.Annotations;

namespace cyberpunk_market_api.src.controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpGet]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Listar avaliações por produto", Description = "Retorna a lista paginada de avaliações do produto. Query: productId, page, pageSize, minRating, maxRating.")]
    public async Task<ActionResult<ApiResponse<PagedResponse<ReviewResponse>>>> GetByProduct(
        [FromQuery] Guid productId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = PaginationConstants.DefaultPageSize,
        [FromQuery] int? minRating = null,
        [FromQuery] int? maxRating = null)
    {
        var result = await _reviewService.GetByProductIdAsync(productId, page, pageSize, minRating, maxRating);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Buscar avaliação por ID", Description = "Busca uma avaliação pelo identificador.")]
    public async Task<ActionResult<ApiResponse<ReviewResponse?>>> GetById(Guid id)
    {
        var result = await _reviewService.GetByIdAsync(id);
        if (!result.success)
            return NotFound(result);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Buyer,Seller")]
    [SwaggerOperation(Summary = "Criar avaliação", Description = "Registra uma avaliação para um produto (usuário autenticado).")]
    public async Task<ActionResult<ApiResponse<ReviewResponse>>> Create([FromBody] CreateReviewDto dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized(ApiResponse<ReviewResponse>.Fail("Usuário não autorizado."));
        var result = await _reviewService.CreateAsync(userId, dto);
        if (!result.success)
            return BadRequest(result);
        return StatusCode(201, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Buyer,Seller")]
    [SwaggerOperation(Summary = "Atualizar avaliação", Description = "Atualiza a própria avaliação do usuário autenticado.")]
    public async Task<ActionResult<ApiResponse<ReviewResponse?>>> Update(Guid id, [FromBody] UpdateReviewDto dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized(ApiResponse<ReviewResponse?>.Fail("Usuário não autorizado."));
        var result = await _reviewService.UpdateAsync(userId, id, dto);
        if (!result.success)
            return BadRequest(result);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Buyer,Seller")]
    [SwaggerOperation(Summary = "Remover avaliação", Description = "Remove a própria avaliação do usuário autenticado.")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized(ApiResponse<object>.Fail("Usuário não autorizado."));
        var result = await _reviewService.DeleteAsync(userId, id);
        if (!result.success)
            return NotFound(result);
        return Ok(result);
    }
}
