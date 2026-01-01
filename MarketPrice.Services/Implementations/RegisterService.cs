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

    public class RegisterService(MarketPriceDbContext context, IPasswordHashService passwordHasherservice) : IRegisterService
    {
        private readonly MarketPriceDbContext _context = context;
        private readonly IPasswordHashService _passwordHasherservice = passwordHasherservice;

        public async Task<RegisterResponseDto> RegisterAsync(RegisterCommand command)
        {
            // validate required fields

            if (string.IsNullOrEmpty(command.FirstName)
                || string.IsNullOrEmpty(command.FamilyName)
                || string.IsNullOrEmpty(command.PhoneNumber)
                || string.IsNullOrEmpty(command.AccountTypeId.ToString())
                || string.IsNullOrEmpty(command.EmailAddress)
                || string.IsNullOrEmpty(command.Password))
            {
                return RegisterResponseDto.Failed("Invalid request data");
            }

            //now check if the user with one of this exists (check if a user already exists with email or phone)
            bool exists = await _context.Users.AnyAsync(u =>
            u.EmailAddress == command.EmailAddress
            || u.PhoneNumber == command.PhoneNumber);

            if (exists)
                return RegisterResponseDto.Failed("A user with email address or phone number already exist");
            //Hash password securely
            var passwordSalt = _passwordHasherservice.GenerateSalt();
            var hashedPassword = _passwordHasherservice.HashPassword(command.Password, passwordSalt);

            //var UserType = 

            // create the user entity
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
            //Save the user info to the database 
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            //Return Success reponse
            return RegisterResponseDto.Succeed(user.EmailAddress, "User Registered successfully");
        }

    }

}


