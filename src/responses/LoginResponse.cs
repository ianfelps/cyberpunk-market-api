using System;

namespace cyberpunk_market_api.src.responses
{
    public class LoginResponse
    {
        public string token { get; set; } = String.Empty;
        public UserResponse user { get; set; } = new UserResponse();
    }
}
