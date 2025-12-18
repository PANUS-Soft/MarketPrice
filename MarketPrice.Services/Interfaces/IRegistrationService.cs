using MarketPrice.Domain.Authentication.Commands;
using MarketPrice.Domain.Authentication.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Services.Interfaces
{
    /// <summary>
    /// here is the contract for the user registration business logic
    /// </summary>
    public interface IRegistrationService
    {
     /// <summary>
     /// checks if a user already exists with the given email or phone number. 
     /// </summary>
     /// <param name="email"></param>
     /// <param name="phoneNumber"></param>
     /// <returns></returns>

        //Task<bool> UserExists(string email, string phoneNumber);


      /// <summary>
      /// Registration create a new user by taking all the command from the MarketPrice Domain>
      /// </summary>
      /// <param name="registrationCommand"></param>
      /// <returns></returns>
        Task<RegisterResponseDto> RegisterAsync(RegistrationCommand Command);

    }
}
