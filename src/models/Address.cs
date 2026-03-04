namespace CyberpunkMarket.Models;

public class Address : BaseEntity
{
    public Guid UserId { get; set; }
    public virtual User User { get; set; } = null!;
    public required string Street { get; set; }
    public required string Number { get; set; }
    public string? Complement { get; set; }
    public required string Neighborhood { get; set; }
    public required string City { get; set; }
    public required string State { get; set; }
    public required string ZipCode { get; set; }
    public bool IsDefault { get; set; }
}
