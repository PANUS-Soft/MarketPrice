using MarketPrice.Data.Models;
using MarketPrice.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace MarketPrice.Services.Implementations
{
    public class TokenService : ITokenService
    {
        private readonly SigningCredentials _credentials;
        private readonly IConfiguration _config;

        // The SigningCredentials (containing the Private Key) is injected from Program.cs
        public TokenService(SigningCredentials credentials, IConfiguration config)
        {
            _credentials = credentials;
            _config = config;
        }

        public string CreateAccessToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.EmailAddress),
                new Claim(ClaimTypes.Role, user.AccountTypeId.ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(10), // Short-lived
                SigningCredentials = _credentials,
                Issuer = _config["Authentication:Issuer"],
                Audience = _config["Authentication:Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string CreateRefreshToken(User user)
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                // We use the Public Key for validation (usually found in the credentials or config)
                IssuerSigningKey = _credentials.Key,
                ValidateLifetime = false, // <--- CRITICAL: Ignore expiration date
                ValidIssuer = _config["Authentication:Issuer"],
                ValidAudience = _config["Authentication:Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.RsaSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }
    }
}
