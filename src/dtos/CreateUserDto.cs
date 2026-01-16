using System;
using System.ComponentModel.DataAnnotations;

namespace cyberpunk_market_api.src.dtos
{
    public class CreateUserDto
    {
        [Required(ErrorMessage = "O username é obrigatório")]
        [MaxLength(25, ErrorMessage = "O username deve ter no máximo 25 caracteres")]
        public string username { get; set; } = String.Empty;

        [Required(ErrorMessage = "O nome completo é obrigatório")]
        [MaxLength(100, ErrorMessage = "O nome completo deve ter no máximo 100 caracteres")]
        public string fullName { get; set; } = String.Empty;

        [Required(ErrorMessage = "O CPF é obrigatório")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "O CPF deve ter exatamente 11 caracteres")]
        public string cpf { get; set; } = String.Empty;

        [Required(ErrorMessage = "A data de nascimento é obrigatória")]
        public DateTime birthDate { get; set; }

        [Required(ErrorMessage = "O email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string email { get; set; } = String.Empty;

        [Required(ErrorMessage = "A senha é obrigatória")]
        [MinLength(8, ErrorMessage = "A senha deve ter no mínimo 8 caracteres")]
        public string password { get; set; } = String.Empty;

        [Required(ErrorMessage = "O role é obrigatório")]
        public int role { get; set; }
    }
}
