using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using cyberpunk_market_api.src.dtos;
using cyberpunk_market_api.src.interfaces;
using cyberpunk_market_api.src.responses;

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
    public async Task<ActionResult<ApiResponse<LoginResponse?>>> Login([FromBody] LoginDto dto)
    {
        var result = await _userService.LoginAsync(dto);
        if (!result.success)
            return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("buyer")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<UserResponse>>> CreateBuyer([FromBody] CreateBuyerDto dto)
    {
        var result = await _userService.CreateBuyerAsync(dto);
        if (!result.success)
            return BadRequest(result);
        return StatusCode(201, result);
    }

    [HttpPost("seller")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<UserResponse>>> CreateSeller([FromBody] CreateSellerDto dto)
    {
        var result = await _userService.CreateSellerAsync(dto);
        if (!result.success)
            return BadRequest(result);
        return StatusCode(201, result);
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<ApiResponse<IEnumerable<UserResponse>>>> GetAll()
    {
        var result = await _userService.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<UserResponse?>>> GetById(Guid id)
    {
        var result = await _userService.GetByIdAsync(id);
        if (!result.success)
            return NotFound(result);
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<UserResponse?>>> Update(Guid id, [FromBody] UpdateUserDto dto)
    {
        var result = await _userService.UpdateAsync(id, dto);
        if (!result.success)
            return BadRequest(result);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id)
    {
        var result = await _userService.DeleteAsync(id);
        if (!result.success)
            return NotFound(result);
        return Ok(result);
    }
}
