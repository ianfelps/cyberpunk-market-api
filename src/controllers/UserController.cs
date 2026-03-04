using CyberpunkMarket.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using cyberpunk_market_api.src.constants;
using cyberpunk_market_api.src.dtos.User;
using cyberpunk_market_api.src.interfaces;
using cyberpunk_market_api.src.responses;
using Swashbuckle.AspNetCore.Annotations;

namespace cyberpunk_market_api.src.controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [EnableRateLimiting("auth")]
    [SwaggerOperation(Summary = "Login do usuário", Description = "Realiza o login de um usuário e retorna o token JWT.")]
    public async Task<ActionResult<ApiResponse<LoginResponse?>>> Login([FromBody] LoginDto dto)
    {
        var result = await _userService.LoginAsync(dto);
        if (!result.success)
            return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("buyer")]
    [AllowAnonymous]
    [EnableRateLimiting("auth")]
    [SwaggerOperation(Summary = "Cadastro de comprador", Description = "Cria um novo usuário com perfil de comprador.")]
    public async Task<ActionResult<ApiResponse<UserResponse>>> CreateBuyer([FromBody] CreateBuyerDto dto)
    {
        var result = await _userService.CreateBuyerAsync(dto);
        if (!result.success)
            return BadRequest(result);
        return StatusCode(201, result);
    }

    [HttpPost("seller")]
    [AllowAnonymous]
    [EnableRateLimiting("auth")]
    [SwaggerOperation(Summary = "Cadastro de vendedor", Description = "Cria um novo usuário com perfil de vendedor.")]
    public async Task<ActionResult<ApiResponse<UserResponse>>> CreateSeller([FromBody] CreateSellerDto dto)
    {
        var result = await _userService.CreateSellerAsync(dto);
        if (!result.success)
            return BadRequest(result);
        return StatusCode(201, result);
    }

    [HttpGet]
    [Authorize]
    [SwaggerOperation(Summary = "Listar usuários", Description = "Retorna a lista paginada de usuários. Filtros: name, email, role. Query: page, pageSize.")]
    public async Task<ActionResult<ApiResponse<PagedResponse<UserResponse>>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = PaginationConstants.DefaultPageSize,
        [FromQuery] string? name = null,
        [FromQuery] string? email = null,
        [FromQuery] UserRole? role = null)
    {
        var result = await _userService.GetAllAsync(page, pageSize, name, email, role);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    [SwaggerOperation(Summary = "Buscar usuário por ID", Description = "Busca um usuário específico pelo seu identificador único.")]
    public async Task<ActionResult<ApiResponse<UserResponse?>>> GetById(Guid id)
    {
        var result = await _userService.GetByIdAsync(id);
        if (!result.success)
            return NotFound(result);
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    [SwaggerOperation(Summary = "Atualizar usuário", Description = "Atualiza os dados de um usuário existente.")]
    public async Task<ActionResult<ApiResponse<UserResponse?>>> Update(Guid id, [FromBody] UpdateUserDto dto)
    {
        var result = await _userService.UpdateAsync(id, dto);
        if (!result.success)
            return BadRequest(result);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    [SwaggerOperation(Summary = "Remover usuário", Description = "Remove um usuário existente pelo seu identificador único.")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id)
    {
        var result = await _userService.DeleteAsync(id);
        if (!result.success)
            return NotFound(result);
        return Ok(result);
    }
}
