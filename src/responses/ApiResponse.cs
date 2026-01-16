using System;
using System.Collections.Generic;

namespace cyberpunk_market_api.src.responses
{
    public class ApiResponse<T>
    {
        public bool success { get; set; }
        public string message { get; set; } = String.Empty;
        public T? data { get; set; }
        public List<string>? errors { get; set; }

        public ApiResponse()
        {
            success = true;
            errors = new List<string>();
        }

        public static ApiResponse<T> Success(T data, string message = "Operação realizada com sucesso")
        {
            return new ApiResponse<T>
            {
                success = true,
                message = message,
                data = data
            };
        }

        public static ApiResponse<T> Fail(string message, List<string>? errors = null)
        {
            return new ApiResponse<T>
            {
                success = false,
                message = message,
                errors = errors ?? new List<string>()
            };
        }
    }
}
