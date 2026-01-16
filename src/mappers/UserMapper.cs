using System;
using cyberpunk_market_api.src.models;
using cyberpunk_market_api.src.dtos;
using cyberpunk_market_api.src.responses;

namespace cyberpunk_market_api.src.mappers
{
    public static class UserMapper
    {
        public static UserResponse ToUserResponse(Users user)
        {
            return new UserResponse
            {
                userId = user.userId,
                username = user.username,
                fullName = user.fullName,
                cpf = user.cpf,
                birthDate = user.birthDate,
                email = user.email,
                role = user.role,
                createdAt = user.createdAt,
                updatedAt = user.updatedAt
            };
        }

        public static Users ToEntity(CreateUserDto dto, string passwordHash)
        {
            return new Users
            {
                username = dto.username,
                fullName = dto.fullName,
                cpf = dto.cpf,
                birthDate = dto.birthDate,
                email = dto.email.ToLower().Trim(),
                passwordHash = passwordHash,
                role = dto.role,
                createdAt = DateTime.UtcNow
            };
        }

        public static void UpdateEntity(Users user, UpdateUserDto dto)
        {
            if (!string.IsNullOrEmpty(dto.username))
                user.username = dto.username;

            if (!string.IsNullOrEmpty(dto.fullName))
                user.fullName = dto.fullName;

            if (!string.IsNullOrEmpty(dto.cpf))
                user.cpf = dto.cpf;

            if (dto.birthDate.HasValue)
                user.birthDate = dto.birthDate.Value;

            if (!string.IsNullOrEmpty(dto.email))
                user.email = dto.email.ToLower().Trim();

            user.updatedAt = DateTime.UtcNow;
        }
    }
}
