namespace cyberpunk_market_api.src.dtos.User;

public class CreateSellerDto
{
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string StoreName { get; set; }
    public string? Bio { get; set; }
}
