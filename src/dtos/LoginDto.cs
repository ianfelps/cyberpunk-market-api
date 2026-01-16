using System;
using System.ComponentModel.DataAnnotations;

namespace cyberpunk_market_api.src.dtos
{
    public class LoginDto
    {
        [Required(ErrorMessage = "O email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string email { get; set; } = String.Empty;

        [Required(ErrorMessage = "A senha é obrigatória")]
        public string password { get; set; } = String.Empty;
    }
}
