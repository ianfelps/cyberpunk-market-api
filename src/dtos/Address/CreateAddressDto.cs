namespace cyberpunk_market_api.src.dtos.Address;

public class CreateAddressDto
{
    public required string Street { get; set; }
    public required string Number { get; set; }
    public string? Complement { get; set; }
    public required string Neighborhood { get; set; }
    public required string City { get; set; }
    public required string State { get; set; }
    public required string ZipCode { get; set; }
    public bool IsDefault { get; set; }
}
