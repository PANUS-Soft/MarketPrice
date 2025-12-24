using MarketPrice.Data;
using MarketPrice.Data.Models;
using MarketPrice.Domain.Authentication.Commands;
using MarketPrice.Domain.Authentication.DTOs;
using MarketPrice.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using LinqToDB;
using System.Security.Principal;


namespace MarketPrice.Services.Implementations
{
    /// <summary>
    /// Handels user Registration using the custom MarketPrice user Model.
    /// </summary>

    public class RegisterService : IRegisterService
    {
        private readonly MarketPriceDbContext _marketPriceDbContext;
        private readonly IPasswordHashService _passwordHasherservice;
        public RegisterService(MarketPriceDbContext marketPriceDbContext, IPasswordHashService passwordHasherservice)
        {
            _marketPriceDbContext = marketPriceDbContext;
            _passwordHasherservice = passwordHasherservice;
        }

        public async Task<RegisterResponseDto> RegisterAsync(RegisterCommand Command)
        {
            // validate required fields
            
            if(string.IsNullOrEmpty(Command.EmailAddress) 
                || string.IsNullOrEmpty(Command.Password))
            {
                return RegisterResponseDto.Failed("Email and Password are required.");
            }

            //now check if the user with one of this exists (check if a user already exists with email or phone)
            bool exists = await _marketPriceDbContext.Users.AnyAsync(u => 
            u.EmailAddress == Command.EmailAddress 
            || u.PhoneNumber == Command.PhoneNumber);

            if (exists)
                return RegisterResponseDto.Failed("A user with email address or phone number already exist");
            //Hash password securely
            var PasswordSalt = _passwordHasherservice.GenerateSalt();
            var hashedPassword = _passwordHasherservice.HashPassword(Command.Password, PasswordSalt);

            //var UserType = 

            // create the user entity
            var user = new User
            {
                FirstName = Command.FirstName,
                FamilyName = Command.FamilyName,
                OtherNames = Command.OtherNames,
                EmailAddress = Command.EmailAddress,
                PhoneNumber = Command.PhoneNumber,
                PasswordHash = hashedPassword,
                AccountTypeId = Command.AccountTypeId,
                IsPremiumUser = false,
                DateRecorded = DateTimeOffset.Now,
                IdCardNumber = null,
                Note = null,

            };
            //Save the user info to the database 
            _marketPriceDbContext.Users.Add(user);
            await _marketPriceDbContext.SaveChangesAsync();

            //Return success reponse
            return RegisterResponseDto.Success(user.EmailAddress, "User Registered successfully");
        }

    }

}

    
