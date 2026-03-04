using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using cyberpunk_market_api.src.constants;
using cyberpunk_market_api.src.dtos.Address;
using cyberpunk_market_api.src.interfaces;
using cyberpunk_market_api.src.responses;
using Swashbuckle.AspNetCore.Annotations;

namespace cyberpunk_market_api.src.controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Buyer,Seller")]
public class AddressController : ControllerBase
{
    private readonly IAddressService _addressService;

    public AddressController(IAddressService addressService)
    {
        _addressService = addressService;
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Listar endereços", Description = "Retorna a lista paginada de endereços do usuário. Filtros: city, zipCode. Query: page, pageSize.")]
    public async Task<ActionResult<ApiResponse<PagedResponse<AddressResponse>>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = PaginationConstants.DefaultPageSize,
        [FromQuery] string? city = null,
        [FromQuery] string? zipCode = null)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized(ApiResponse<PagedResponse<AddressResponse>>.Fail("Usuário não autorizado."));
        var result = await _addressService.GetAllByUserAsync(userId, page, pageSize, city, zipCode);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [SwaggerOperation(Summary = "Buscar endereço por ID", Description = "Busca um endereço do usuário pelo identificador.")]
    public async Task<ActionResult<ApiResponse<AddressResponse?>>> GetById(Guid id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized(ApiResponse<AddressResponse?>.Fail("Usuário não autorizado."));
        var result = await _addressService.GetByIdAsync(userId, id);
        if (!result.success)
            return NotFound(result);
        return Ok(result);
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Cadastrar endereço", Description = "Cadastra um novo endereço para o usuário autenticado.")]
    public async Task<ActionResult<ApiResponse<AddressResponse>>> Create([FromBody] CreateAddressDto dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized(ApiResponse<AddressResponse>.Fail("Usuário não autorizado."));
        var result = await _addressService.CreateAsync(userId, dto);
        if (!result.success)
            return BadRequest(result);
        return StatusCode(201, result);
    }

    [HttpPut("{id:guid}")]
    [SwaggerOperation(Summary = "Atualizar endereço", Description = "Atualiza um endereço do usuário autenticado.")]
    public async Task<ActionResult<ApiResponse<AddressResponse?>>> Update(Guid id, [FromBody] UpdateAddressDto dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized(ApiResponse<AddressResponse?>.Fail("Usuário não autorizado."));
        var result = await _addressService.UpdateAsync(userId, id, dto);
        if (!result.success)
            return BadRequest(result);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [SwaggerOperation(Summary = "Remover endereço", Description = "Remove um endereço do usuário autenticado.")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized(ApiResponse<object>.Fail("Usuário não autorizado."));
        var result = await _addressService.DeleteAsync(userId, id);
        if (!result.success)
            return BadRequest(result);
        return Ok(result);
    }
}
