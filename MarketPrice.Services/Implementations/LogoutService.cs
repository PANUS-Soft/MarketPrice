using MarketPrice.Data;
using MarketPrice.Domain.Authentication.Commands;
using MarketPrice.Domain.Authentication.DTOs;
using MarketPrice.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MarketPrice.Services.Implementations
{
    public class LogoutService(MarketPriceDbContext context) : ILogoutService
    {
        public async Task<LogoutResponseDto> LogoutAsync(LogoutCommand command)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.EmailAddress == command.EmailAddress);

            if (user != null)
            {
                var security = await context.UserSecurityDetails.FirstOrDefaultAsync(s => s.UserId == user.UserId);
                if (security != null)
                {
                    // Invalidate the refresh token so it cannot be used again
                    security.RefreshToken = null;
                    security.RefreshTokenExpiryTime = null;
                    await context.SaveChangesAsync();
                }
            }

            return new LogoutResponseDto { LogoutStatus = true };
        }

    }
}
