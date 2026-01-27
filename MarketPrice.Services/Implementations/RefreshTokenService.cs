using MarketPrice.Data;
using MarketPrice.Domain.Authentication;
using MarketPrice.Domain.Authentication.Commands;
using MarketPrice.Domain.Authentication.DTOs;
using MarketPrice.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Services.Implementations
{
    public class RefreshTokenService(MarketPriceDbContext context, ITokenService tokenService) : IRefreshTokenService
    {
        public async Task<AuthenticationResponseDto> RefreshTokenAsync(RefreshTokenCommand command)
        {
            var securityDetail = await context.UserSecurityDetails.FirstOrDefaultAsync(x => x.RefreshToken == command.RefreshToken);
            if (securityDetail == null)
                return DtoManager.Failed<AuthenticationResponseDto>("Invalid refresh token");

            if (securityDetail.UserId != command.UserId)
                return DtoManager.Failed<AuthenticationResponseDto>("Token mismatch ... Refresh token does not match the user");

            if (securityDetail.RefreshTokenExpiryTime < DateTime.Now)
                return DtoManager.Failed<AuthenticationResponseDto>("Refresh token expired");

            var user = context.Users.FirstOrDefault(u => u.UserId == command.UserId);

            if (user == null)
                return DtoManager.Failed<AuthenticationResponseDto>("User not found");

            var newAccessToken = tokenService.CreateAccessToken(user);
            var newRefreshToken = tokenService.CreateRefreshToken(user);

            securityDetail.RefreshToken = newRefreshToken;
            securityDetail.LastActivityDate = DateTime.UtcNow;

            await context.SaveChangesAsync();

            var dto = new AuthenticationResponseDto()
            {
                UserId = user.UserId,
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                ExpiryDate = DateTime.UtcNow.AddMinutes(10)
            };
            return DtoManager.Succeed(dto);
        }
    }
}
