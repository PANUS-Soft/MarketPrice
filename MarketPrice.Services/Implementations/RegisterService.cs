using MarketPrice.Data;
using MarketPrice.Data.Models;
using MarketPrice.Domain.Authentication.Commands;
using MarketPrice.Domain.Authentication.DTOs;
using MarketPrice.Services.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace MarketPrice.Services.Implementations
{
    /// <summary>
    /// Handels user Registration using the custom MarketPrice user Model.
    /// </summary>

    public class RegisterService(MarketPriceDbContext context, IPasswordHashService passwordHasherservice, ITokenService tokenService) : IRegisterService
    {
        private readonly MarketPriceDbContext _context = context;
        private readonly ITokenService _tokenService = tokenService;
        private readonly IPasswordHashService _passwordHasherservice = passwordHasherservice;

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
                return RegisterResponseDto.Failed("Invalid request data");
            }

            // Now check if the user with one of this exists (check if a user already exists with email or phone)

            bool userExists = await _context.Users.AnyAsync(u => u.EmailAddress == command.EmailAddress || u.PhoneNumber == command.PhoneNumber);

            if (userExists)
                return RegisterResponseDto.Failed("An account alreay exists with the provided email address or phone number.");

            // Hash password securely
            var passwordSalt = _passwordHasherservice.GenerateSalt();
            var hashedPassword = _passwordHasherservice.HashPassword(command.Password, passwordSalt);

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
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

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
            _context.UserSecurityDetails.Add(security);
            await _context.SaveChangesAsync();

            var expiryDate = DateTime.Now.AddMinutes(10); 

            // Return Success reponse
            return RegisterResponseDto.Succeed(user.EmailAddress, "User created successfully", user.FirstName, accessToken, refreshToken, expiryDate);
        }

    }

}


