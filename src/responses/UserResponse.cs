using System;

namespace cyberpunk_market_api.src.responses
{
    public class UserResponse
    {
        public int userId { get; set; }
        public string username { get; set; } = String.Empty;
        public string fullName { get; set; } = String.Empty;
        public string cpf { get; set; } = String.Empty;
        public DateTime birthDate { get; set; }
        public string email { get; set; } = String.Empty;
        public int role { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime? updatedAt { get; set; }
    }
}
