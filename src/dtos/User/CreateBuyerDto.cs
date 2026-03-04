namespace cyberpunk_market_api.src.dtos.User;

public class CreateBuyerDto
{
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
}
