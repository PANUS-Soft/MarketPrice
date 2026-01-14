using MarketPrice.Domain.Authentication.Commands;
using MarketPrice.Domain.Authentication.DTOs;

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

    }
}
