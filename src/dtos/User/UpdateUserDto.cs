using CyberpunkMarket.Models;

namespace cyberpunk_market_api.src.dtos.User;

public class UpdateUserDto
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public UserRole? Role { get; set; }
}
