using CyberpunkMarket.Models;
using cyberpunk_market_api.src.dtos.Address;
using cyberpunk_market_api.src.responses;

namespace cyberpunk_market_api.src.mappers;

public static class AddressMapper
{
    public static AddressResponse ToResponse(Address address)
    {
        return new AddressResponse
        {
            Id = address.Id,
            Street = address.Street,
            Number = address.Number,
            Complement = address.Complement,
            Neighborhood = address.Neighborhood,
            City = address.City,
            State = address.State,
            ZipCode = address.ZipCode,
            IsDefault = address.IsDefault,
            CreatedAt = address.CreatedAt,
            UpdatedAt = address.UpdatedAt
        };
    }

    public static Address ToEntity(CreateAddressDto dto, Guid userId)
    {
        return new Address
        {
            UserId = userId,
            Street = dto.Street,
            Number = dto.Number,
            Complement = dto.Complement,
            Neighborhood = dto.Neighborhood,
            City = dto.City,
            State = dto.State,
            ZipCode = dto.ZipCode,
            IsDefault = dto.IsDefault
        };
    }
}
