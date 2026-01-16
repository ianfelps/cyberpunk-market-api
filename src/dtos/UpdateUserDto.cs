using System;
using System.ComponentModel.DataAnnotations;

namespace cyberpunk_market_api.src.dtos
{
    public class UpdateUserDto
    {
        [MaxLength(25, ErrorMessage = "O username deve ter no máximo 25 caracteres")]
        public string? username { get; set; }

        [MaxLength(100, ErrorMessage = "O nome completo deve ter no máximo 100 caracteres")]
        public string? fullName { get; set; }

        [StringLength(11, MinimumLength = 11, ErrorMessage = "O CPF deve ter exatamente 11 caracteres")]
        public string? cpf { get; set; }

        public DateTime? birthDate { get; set; }

        [EmailAddress(ErrorMessage = "Email inválido")]
        public string? email { get; set; }
    }
}
