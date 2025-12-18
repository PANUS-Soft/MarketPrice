using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Domain.Authentication.DTOs
{
    public class RegisterResponseDto
    {

        public bool success { get; set; }
        public string EmailAddress { get; set; } = string.Empty;
        public string CreationStatus { get; set; } = string.Empty;
        public IEnumerable<string> Errors { get; set; }  = new List<string>();

        /// <summary>
        /// User successfuly register to the platform
        /// </summary>
        /// <param name="email"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static RegisterResponseDto Success(string email, string message = "User created successfully!")
        {
            return new RegisterResponseDto
            {
                EmailAddress = email,
                CreationStatus = message,
                success = true
            };
        }

        /// <summary>
        /// if the user is already existing in the system
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <returns></returns>

        public static RegisterResponseDto Failed(string errorMessage = "User with this EmailAddress already existe!")
        {
            return new RegisterResponseDto
            {
                EmailAddress = string.Empty,
                CreationStatus = errorMessage,  
                success = false,
            };
        }
        /// <summary>
        /// A list of errors that occurred during registration.
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        public static RegisterResponseDto Failed(IEnumerable<string> error)
        {
            return new RegisterResponseDto
            {
                success = false,
                Errors = error,
                CreationStatus = "Registration failed",
                
            };

        }
    }
}
