using MarketPrice.Data;
using MarketPrice.Data.Models;
using MarketPrice.Domain.Authentication.Commands;
using MarketPrice.Domain.Authentication.DTOs;
using MarketPrice.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MarketPrice.Services.Implementations
{
    public class LoginService(
        MarketPriceDbContext context,
        IPasswordHashService hashService,
        ITokenService tokenService)
        : ILoginService
    {
        // Your EF Core DB Context

        private readonly MarketPriceDbContext _context = context;
        private readonly IPasswordHashService _hashService = hashService;

        public async Task<LoginResponseDto> LoginAsync(LoginCommand command)
        {
            // 1. Find User
            var user = await _context.Users.FirstOrDefaultAsync(u => u.EmailAddress == command.EmailAddress);
            if (user == null)
                return new LoginResponseDto
                {
                    Success = false,
                    LoginStatus = "The email or password you entered is incorrect"
                };

            // 2. Verify Password
            bool isValid = _hashService.VerifyPassword(command.Password, user.PasswordHash, user.PasswordSalt);
            if (!isValid)
                return new LoginResponseDto
                {
                    Success = false,
                    LoginStatus = "The email or password you entered is incorrect"
                };

            // 3. Generate Tokens
            var accessToken = tokenService.CreateAccessToken(user);
            var refreshToken = tokenService.CreateRefreshToken(user);

            // 4. APPLY "REMEMBER ME" LOGIC
            // If RememberMe is true, token lasts 6 months. Otherwise, 7 days.
            DateTime refreshTokenExpiry = command.RememberMe
                ? DateTime.UtcNow.AddMonths(6)
                : DateTime.UtcNow.AddDays(7);


            var security = await _context.UserSecurityDetails.FirstOrDefaultAsync(s => s.UserId == user.UserId);

            if (security == null)
            {
                // Create new record if first time logging in or record was deleted
                security = new UserSecurityDetail()
                {
                    UserId = user.UserId,
                    RefreshToken = refreshToken,
                    RefreshTokenExpiryTime = refreshTokenExpiry,
                    LastActivityDate = DateTime.Now
                };
                _context.UserSecurityDetails.Add(security);
            }
            else
            {
                // Update existing record (IsUnique constraint ensures only one exists)
                security.RefreshToken = refreshToken;
                security.RefreshTokenExpiryTime = refreshTokenExpiry;
                security.LastActivityDate = DateTime.UtcNow;
            }
            await _context.SaveChangesAsync();

            // 6. Return Data to Client
            return new LoginResponseDto
            {
                FirstName = user.FirstName,
                FamilyName = user.FamilyName,
                EmailAddress = user.EmailAddress,
                PhoneNumber = user.PhoneNumber,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiryDate = DateTime.UtcNow.AddMinutes(10), // Access token expiry
                Success = true,
                LoginStatus = "User logged in successfully"
            };
        }
    }

}
