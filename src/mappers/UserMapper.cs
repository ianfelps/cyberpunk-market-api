using CyberpunkMarket.Models;
using cyberpunk_market_api.src.dtos.User;
using cyberpunk_market_api.src.responses;

namespace cyberpunk_market_api.src.mappers;

public static class UserMapper
{
    public static UserResponse ToResponse(User user)
    {
        return new UserResponse
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role,
            CreatedAt = user.CreatedAt
        };
    }

    public static User ToEntity(CreateBuyerDto dto, string passwordHash)
    {
        return new User
        {
            Name = dto.Name,
            Email = dto.Email,
            PasswordHash = passwordHash,
            Role = UserRole.Buyer
        };
    }

    public static User ToEntity(CreateSellerDto dto, string passwordHash)
    {
        return new User
        {
            Name = dto.Name,
            Email = dto.Email,
            PasswordHash = passwordHash,
            Role = UserRole.Seller
        };
    }
}
