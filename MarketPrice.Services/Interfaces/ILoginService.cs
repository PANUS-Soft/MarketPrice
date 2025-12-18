using MarketPrice.Domain.Authentication.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarketPrice.Domain.Authentication.Commands;

namespace MarketPrice.Services.Interfaces
{
     public interface ILoginService
     {
        /// <summary>
        /// Handles user authentication, password verification and token issuance.
        /// </summary>
        /// <param name="command">The login request data.</param>
        /// <returns>A DTO containing tokens, user details, and status.</returns>
        Task<LoginResponseDto> LoginAsync(LoginCommand command);

        /// <summary>
        /// Revokes the user's refresh token, effectively logging them out of all sessions. 
        /// </summary>
        /// <param name="command"> The logout request data, containing the user's email.</param>
        /// <returns> A Dto indicating the status of the logout attempt. </returns>
        Task<LogoutResponseDto> LogoutAsync(LogoutCommand command);
     }
}
