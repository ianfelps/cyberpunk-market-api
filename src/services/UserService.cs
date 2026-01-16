using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using cyberpunk_market_api.src.contexts;
using cyberpunk_market_api.src.dtos;
using cyberpunk_market_api.src.interfaces;
using cyberpunk_market_api.src.mappers;
using cyberpunk_market_api.src.models;
using cyberpunk_market_api.src.responses;

namespace cyberpunk_market_api.src.services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtService _jwtService;

        public UserService(ApplicationDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        public async Task<List<UserResponse>> GetAllUsers()
        {
            var users = await _context.users.ToListAsync();
            return users.Select(u => UserMapper.ToUserResponse(u)).ToList();
        }

        public async Task<UserResponse?> GetUserById(int id)
        {
            var user = await _context.users.FindAsync(id);
            return user == null ? null : UserMapper.ToUserResponse(user);
        }

        public async Task<UserResponse?> GetUserByEmail(string email)
        {
            var user = await _context.users
                .FirstOrDefaultAsync(u => u.email == email.ToLower().Trim());
            return user == null ? null : UserMapper.ToUserResponse(user);
        }

        public async Task<UserResponse> CreateUser(CreateUserDto dto)
        {
            // Verifica se o email já existe
            var existingUser = await _context.users
                .FirstOrDefaultAsync(u => u.email == dto.email.ToLower().Trim());

            if (existingUser != null)
                throw new InvalidOperationException("Email já cadastrado");

            // Verifica se o username já existe
            var existingUsername = await _context.users
                .FirstOrDefaultAsync(u => u.username == dto.username);

            if (existingUsername != null)
                throw new InvalidOperationException("Username já cadastrado");

            // Verifica se o CPF já existe
            var existingCpf = await _context.users
                .FirstOrDefaultAsync(u => u.cpf == dto.cpf);

            if (existingCpf != null)
                throw new InvalidOperationException("CPF já cadastrado");

            // Hash da senha usando BCrypt
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.password);

            // Cria a entidade
            var user = UserMapper.ToEntity(dto, passwordHash);

            // Salva no banco
            _context.users.Add(user);
            await _context.SaveChangesAsync();

            return UserMapper.ToUserResponse(user);
        }

        public async Task<UserResponse?> UpdateUser(int id, UpdateUserDto dto)
        {
            var user = await _context.users.FindAsync(id);
            if (user == null)
                return null;

            // Verifica se o novo email já existe (se fornecido)
            if (!string.IsNullOrEmpty(dto.email) && dto.email.ToLower().Trim() != user.email)
            {
                var existingEmail = await _context.users
                    .FirstOrDefaultAsync(u => u.email == dto.email.ToLower().Trim());

                if (existingEmail != null)
                    throw new InvalidOperationException("Email já cadastrado");
            }

            // Verifica se o novo username já existe (se fornecido)
            if (!string.IsNullOrEmpty(dto.username) && dto.username != user.username)
            {
                var existingUsername = await _context.users
                    .FirstOrDefaultAsync(u => u.username == dto.username);

                if (existingUsername != null)
                    throw new InvalidOperationException("Username já cadastrado");
            }

            // Verifica se o novo CPF já existe (se fornecido)
            if (!string.IsNullOrEmpty(dto.cpf) && dto.cpf != user.cpf)
            {
                var existingCpf = await _context.users
                    .FirstOrDefaultAsync(u => u.cpf == dto.cpf);

                if (existingCpf != null)
                    throw new InvalidOperationException("CPF já cadastrado");
            }

            // Atualiza os campos
            UserMapper.UpdateEntity(user, dto);

            await _context.SaveChangesAsync();

            return UserMapper.ToUserResponse(user);
        }

        public async Task<bool> DeleteUser(int id)
        {
            var user = await _context.users.FindAsync(id);
            if (user == null)
                return false;

            _context.users.Remove(user);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<LoginResponse?> Login(LoginDto dto)
        {
            // Busca o usuário pelo email
            var user = await _context.users
                .FirstOrDefaultAsync(u => u.email == dto.email.ToLower().Trim());

            if (user == null)
                return null;

            // Verifica a senha
            bool isValidPassword = BCrypt.Net.BCrypt.Verify(dto.password, user.passwordHash);
            if (!isValidPassword)
                return null;

            // Gera o token JWT
            string token = _jwtService.GenerateToken(user);

            return new LoginResponse
            {
                token = token,
                user = UserMapper.ToUserResponse(user)
            };
        }
    }
}
