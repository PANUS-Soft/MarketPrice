using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MarketPrice.Services.Interfaces;
using MarketPrice.Data;
using MarketPrice.Domain.ChangePassword.Dto;
using MarketPrice.Domain.ChangePassword.Command;

namespace MarketPrice.Services.Implementations
{
    public class ChangePasswordService : IUserSecurityService
    {
        private readonly MarketPriceDbContext _context;
        private readonly IPasswordHashService _passwordHashService;

        public ChangePasswordService(
            MarketPriceDbContext marketPriceDbContext,
            IPasswordHashService passwordHashService)
        {
            _context = marketPriceDbContext;
            _passwordHashService = passwordHashService;
        }

        public async Task<ChangePasswordResponseDto> ChangePasswordAsync(Guid userId, ChangePasswordCommand command)
        {
            var security = await _context.Users
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (security == null)
            {
                return new ChangePasswordResponseDto
                {
                    Success = false,
                    Message = "User security record not found"
                };
            }

            bool validPassword = _passwordHashService.VerifyPassword(
                command.CurrentPassword,
                security.PasswordHash,
                security.PasswordSalt);

            if (!validPassword)
            {
                return new ChangePasswordResponseDto
                {
                    Success = false,
                    Message = "Current password is incorrect"
                };
            }

            if (command.NewPassword != command.ConfirmPassword)
            {
                return new ChangePasswordResponseDto
                {
                    Success = false,
                    Message = "Passwords do not match"
                };
            }

            string newSalt = _passwordHashService.GenerateSalt();
            string newHash = _passwordHashService.HashPassword(command.NewPassword, newSalt);

            security.PasswordSalt = newSalt;
            security.PasswordHash = newHash;

            await _context.SaveChangesAsync();

            return new ChangePasswordResponseDto
            {
                Success = true,
                Message = "Password changed successfully"
            };
        }
    }
}