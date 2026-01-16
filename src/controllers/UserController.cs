using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using cyberpunk_market_api.src.interfaces;
using cyberpunk_market_api.src.dtos;
using cyberpunk_market_api.src.responses;

namespace cyberpunk_market_api.src.controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse<UserResponse>>> Register([FromBody] CreateUserDto dto)
        {
            try
            {
                var user = await _userService.CreateUser(dto);
                return Ok(ApiResponse<UserResponse>.Success(user, "Usuário criado com sucesso"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<UserResponse>.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<UserResponse>.Fail("Erro interno do servidor", new List<string> { ex.Message }));
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<LoginResponse>>> Login([FromBody] LoginDto dto)
        {
            try
            {
                var result = await _userService.Login(dto);

                if (result == null)
                    return Unauthorized(ApiResponse<LoginResponse>.Fail("Email ou senha inválidos"));

                Response.Cookies.Append("accessToken", result.token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddHours(8)
                });

                return Ok(ApiResponse<LoginResponse>.Success(result, "Login realizado com sucesso"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<LoginResponse>.Fail("Erro interno do servidor", new List<string> { ex.Message }));
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<ApiResponse<List<UserResponse>>>> GetAll()
        {
            try
            {
                var users = await _userService.GetAllUsers();
                return Ok(ApiResponse<List<UserResponse>>.Success(users, "Usuários recuperados com sucesso"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<List<UserResponse>>.Fail("Erro interno do servidor", new List<string> { ex.Message }));
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<UserResponse>>> GetById(int id)
        {
            try
            {
                var user = await _userService.GetUserById(id);

                if (user == null)
                    return NotFound(ApiResponse<UserResponse>.Fail("Usuário não encontrado"));

                return Ok(ApiResponse<UserResponse>.Success(user, "Usuário recuperado com sucesso"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<UserResponse>.Fail("Erro interno do servidor", new List<string> { ex.Message }));
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<UserResponse>>> Update(int id, [FromBody] UpdateUserDto dto)
        {
            try
            {
                var user = await _userService.UpdateUser(id, dto);

                if (user == null)
                    return NotFound(ApiResponse<UserResponse>.Fail("Usuário não encontrado"));

                return Ok(ApiResponse<UserResponse>.Success(user, "Usuário atualizado com sucesso"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<UserResponse>.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<UserResponse>.Fail("Erro interno do servidor", new List<string> { ex.Message }));
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
        {
            try
            {
                var result = await _userService.DeleteUser(id);

                if (!result)
                    return NotFound(ApiResponse<bool>.Fail("Usuário não encontrado"));

                return Ok(ApiResponse<bool>.Success(true, "Usuário deletado com sucesso"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<bool>.Fail("Erro interno do servidor", new List<string> { ex.Message }));
            }
        }

        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("accessToken");
            return Ok(ApiResponse<object>.Success(new { }, "Logout realizado com sucesso"));
        }
    }
}
