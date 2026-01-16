using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace cyberpunk_market_api.src.models
{
    [Table("TB_Users")]
    public class Users
    {
        [Key]
        [Required]
        public int userId { get; set; }
        [Required]
        public string username { get; set; } = String.Empty;
        [Required]
        public string fullName { get; set; } = String.Empty;
        [Required]
        public string cpf { get; set; } = String.Empty;
        [Required]
        public DateTime birthDate { get; set; }
        [Required]
        public string email { get; set; } = String.Empty;
        [Required]
        public string passwordHash { get; set; } = String.Empty;
        [Required]
        public int role { get; set; }
        [Required]
        public DateTime createdAt { get; set; } = DateTime.UtcNow;
        public DateTime? updatedAt { get; set; }

    }
}