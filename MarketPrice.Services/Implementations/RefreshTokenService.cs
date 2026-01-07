using MarketPrice.Data;
using MarketPrice.Domain.Authentication.Commands;
using MarketPrice.Domain.Authentication.DTOs;
using MarketPrice.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Services.Implementations
{
    public class RefreshTokenService(MarketPriceDbContext context, ITokenService tokenService) : IRefreshTokenService
    {
        private readonly MarketPriceDbContext _context = context;
        private readonly ITokenService _tokenService = tokenService;

        public async Task<RefreshTokenResponseDto> RefreshTokenAsync(RefreshTokenCommand command)
        {
            var securityDetail = await _context.UserSecurityDetails.FirstOrDefaultAsync(x => x.RefreshToken == command.RefreshToken);
            if (securityDetail == null)
                return RefreshTokenResponseDto.Failed("Invalid refresh token");

            if (securityDetail.RefreshTokenExpiryTime < DateTime.Now)
                return RefreshTokenResponseDto.Failed("Refresh token expired");

            var user = _context.Users.FirstOrDefault(u => u.UserId == securityDetail.UserId);

            if (user == null)
                return RefreshTokenResponseDto.Failed("User not found");

            var newAccessToken = _tokenService.CreateAccessToken(user);
            var newRefreshToken = _tokenService.CreateRefreshToken(user);

            securityDetail.RefreshToken = newRefreshToken;
            securityDetail.LastActivityDate = DateTime.Now;

            await _context.SaveChangesAsync();

            return RefreshTokenResponseDto.Succeed(newAccessToken, newRefreshToken, DateTime.Now.AddMinutes(10));
        }
    }
}
