using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using cyberpunk_market_api.src.dtos;
using cyberpunk_market_api.src.interfaces;
using cyberpunk_market_api.src.responses;
using Swashbuckle.AspNetCore.Annotations;

namespace cyberpunk_market_api.src.controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    [Authorize(Roles = "Buyer,Seller")]
    [SwaggerOperation(Summary = "Listar produtos", Description = "Retorna a lista de produtos disponíveis para compradores e vendedores.")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ProductResponse>>>> GetAll()
    {
        var result = await _productService.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Buyer,Seller")]
    [SwaggerOperation(Summary = "Buscar produto por ID", Description = "Busca um produto específico pelo seu identificador único.")]
    public async Task<ActionResult<ApiResponse<ProductResponse?>>> GetById(Guid id)
    {
        var result = await _productService.GetByIdAsync(id);
        if (!result.success)
            return NotFound(result);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Seller")]
    [SwaggerOperation(Summary = "Criar produto", Description = "Cria um novo produto vinculado ao vendedor autenticado.")]
    public async Task<ActionResult<ApiResponse<ProductResponse>>> Create([FromBody] CreateProductDto dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized(ApiResponse<ProductResponse>.Fail("Usuário não autorizado."));
        var result = await _productService.CreateAsync(userId, dto);
        if (!result.success)
            return BadRequest(result);
        return StatusCode(201, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Seller")]
    [SwaggerOperation(Summary = "Atualizar produto", Description = "Atualiza os dados de um produto pertencente ao vendedor autenticado.")]
    public async Task<ActionResult<ApiResponse<ProductResponse?>>> Update(Guid id, [FromBody] UpdateProductDto dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized(ApiResponse<ProductResponse?>.Fail("Usuário não autorizado."));
        var result = await _productService.UpdateAsync(userId, id, dto);
        if (!result.success)
            return BadRequest(result);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Seller")]
    [SwaggerOperation(Summary = "Remover produto", Description = "Remove um produto pertencente ao vendedor autenticado.")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized(ApiResponse<object>.Fail("Usuário não autorizado."));
        var result = await _productService.DeleteAsync(userId, id);
        if (!result.success)
            return NotFound(result);
        return Ok(result);
    }
}
