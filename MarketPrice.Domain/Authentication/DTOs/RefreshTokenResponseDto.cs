using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Domain.Authentication.DTOs
{
    public class RefreshTokenResponseDto
    {
        public string AccessToken { get; init; }
        public string RefreshToken { get; init; }
        public DateTime ExpiryDate { get; init; }
        public bool Success { get; init; }
        public string? Message { get; init; }

        public static RefreshTokenResponseDto Failed(string message) =>
            new()
            {
                Success = false,
                Message = message
            };

        public static RefreshTokenResponseDto Succeed(string accessToken, string refreshToken, DateTime expiryDate) =>
            new()
            {
                Success = true,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiryDate = expiryDate
            };
    }
}
