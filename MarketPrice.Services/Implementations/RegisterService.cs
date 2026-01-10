using MarketPrice.Data;
using MarketPrice.Data.Models;
using MarketPrice.Domain.Authentication.Commands;
using MarketPrice.Domain.Authentication.DTOs;
using MarketPrice.Services.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace MarketPrice.Services.Implementations
{
    /// <summary>
    /// Handles user Registration using the custom MarketPrice user Model.
    /// </summary>

    public class RegisterService(MarketPriceDbContext context, IPasswordHashService passwordHasherService, ITokenService tokenService) : IRegisterService
    {
        private readonly ITokenService _tokenService = tokenService;
        private readonly IPasswordHashService _passwordHasherService = passwordHasherService;

        public async Task<RegisterResponseDto> RegisterAsync(RegisterCommand command)
        {
            // Validate required fields

            if (string.IsNullOrEmpty(command.FirstName)
                || string.IsNullOrEmpty(command.FamilyName)
                || string.IsNullOrEmpty(command.PhoneNumber)
                || string.IsNullOrEmpty(command.AccountTypeId.ToString())
                || string.IsNullOrEmpty(command.EmailAddress)
                || string.IsNullOrEmpty(command.Password))
            {
                return DtoManager.Failed<RegisterResponseDto>("Invalid request data");
            }

            // Now check if the user with one of this exists (check if a user already exists with email or phone)

            bool userExists = await context.Users.AnyAsync(u => u.EmailAddress == command.EmailAddress || u.PhoneNumber == command.PhoneNumber);

            if (userExists)
                return DtoManager.Failed<RegisterResponseDto>("An account already exists with the provided email address or phone number.");

            // Hash password securely
            var passwordSalt = _passwordHasherService.GenerateSalt();
            var hashedPassword = _passwordHasherService.HashPassword(command.Password, passwordSalt);

            // Create the user entity
            var user = new User
            {
                FirstName = command.FirstName,
                FamilyName = command.FamilyName,
                OtherNames = command.OtherNames,
                EmailAddress = command.EmailAddress,
                PhoneNumber = command.PhoneNumber,
                PasswordHash = hashedPassword,
                AccountTypeId = command.AccountTypeId,
                IsPremiumUser = false,
                DateRecorded = DateTimeOffset.Now,
                PasswordSalt = passwordSalt,
                IdCardNumber = null,
                Note = null,

            };
            // Save the user info to the database 
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var accessToken = _tokenService.CreateAccessToken(user);
            var refreshToken = _tokenService.CreateRefreshToken(user);
            DateTime refreshTokenExpiry = DateTime.Now.AddMonths(1);

            var security = new UserSecurityDetail()
            {
                UserId = user.UserId,
                RefreshToken = refreshToken,
                RefreshTokenExpiryTime = refreshTokenExpiry,
                LastActivityDate = DateTime.Now
            };

            // Save security details to the database
            context.UserSecurityDetails.Add(security);
            await context.SaveChangesAsync();

            var expiryDate = DateTime.Now.AddMinutes(10); 

            // Return Success response
            var responseDto = new RegisterResponseDto
            {
                FirstName = user.FirstName,
                EmailAddress = user.EmailAddress,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiryDate = expiryDate,
                Status = "User created successfully"
            };
            return DtoManager.Succeed(responseDto);
        }

    }

}


