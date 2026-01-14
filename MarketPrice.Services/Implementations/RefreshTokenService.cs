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
        private readonly MarketPriceDbContext _context = context;
        private readonly ITokenService _tokenService = tokenService;

        public async Task<AuthenticationResponseDto> RefreshTokenAsync(RefreshTokenCommand command)
        {
            //ClaimsPrincipal principal;

            //try
            //{
            //    principal = _tokenService.GetPrincipalFromExpiredToken(command.AccessToken);
            //}
            //catch
            //{
            //    return RefreshTokenResponseDto.Failed("Invalid access token");
            //}

            //var userIdClaim = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            //if (userIdClaim == null)
            //    return RefreshTokenResponseDto.Failed("Invalid token claims");

            //var userId = Guid.Parse(userIdClaim.Value);

            var securityDetail = await _context.UserSecurityDetails.FirstOrDefaultAsync(x => x.RefreshToken == command.RefreshToken);
            if (securityDetail == null)
                return DtoManager.Failed<AuthenticationResponseDto>("Invalid refresh token");

            if (securityDetail.UserId != command.UserId)
                return DtoManager.Failed<AuthenticationResponseDto>("Token mismatch ... Refresh token does not match the user");

            if (securityDetail.RefreshTokenExpiryTime < DateTime.Now)
                return DtoManager.Failed<AuthenticationResponseDto>("Refresh token expired");

            var user = _context.Users.FirstOrDefault(u => u.UserId == command.UserId);

            if (user == null)
                return DtoManager.Failed<AuthenticationResponseDto>("User not found");

            var newAccessToken = _tokenService.CreateAccessToken(user);
            var newRefreshToken = _tokenService.CreateRefreshToken(user);

            securityDetail.RefreshToken = newRefreshToken;
            securityDetail.LastActivityDate = DateTime.Now;

            await _context.SaveChangesAsync();

            var dto = new AuthenticationResponseDto()
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                ExpiryDate = DateTime.Now.AddMinutes(10)
            };
            return DtoManager.Succeed(dto );
        }
    }
}
