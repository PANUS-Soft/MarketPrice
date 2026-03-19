using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MarketPrice.Services.Interfaces;
using MarketPrice.Data;
using MarketPrice.Domain.Profile.Commands;
using MarketPrice.Domain.Profile.DTOs;

namespace MarketPrice.Services.Implementations
{
    public class ChangePasswordService(MarketPriceDbContext context, PasswordHashService passwordHashService) : IChangePasswordService
    {

        public async Task<ChangePasswordResponseDto> ChangePasswordAsync(ChangePasswordCommand command)
        {
            var user = await context.Users
                .FirstOrDefaultAsync(x => x.UserId == command.UserId);

            if (user == null)
            {
                return new ChangePasswordResponseDto
                {
                    Success = false,
                    Message = "User was not found"
                };
            }

            if (command.CurrentPassword == command.NewPassword)
            {
                return new ChangePasswordResponseDto
                {
                    Success = false,
                    Message = "New password cannot be the same as the current password"
                };
            }

            bool validPassword = passwordHashService.VerifyPassword(
                command.CurrentPassword,
                user.PasswordHash,
                user.PasswordSalt);

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

            string newSalt = passwordHashService.GenerateSalt();
            string newHash = passwordHashService.HashPassword(command.NewPassword, newSalt);

            user.PasswordSalt = newSalt;
            user.PasswordHash = newHash;

            await context.SaveChangesAsync();

            return new ChangePasswordResponseDto
            {
                Success = true,
                Message = "Password changed successfully"
            };
        }
    }
}

