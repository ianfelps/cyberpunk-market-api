using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace cyberpunk_market_api.src.models
{
    [Table("TB_UserRoles")]
    public class UserRoles
    {
        [Key]
        [Required]
        public int roleId { get; set; }
        [Required]
        public string name { get; set; } = String.Empty;
    }
}