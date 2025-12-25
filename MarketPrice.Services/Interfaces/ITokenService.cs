using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using MarketPrice.Data.Models;

namespace MarketPrice.Services.Interfaces
{
    public interface ITokenService
    {
        /// <summary>
        /// Creates a signed JWT Access Token using the asymmetric private key.
        /// </summary>
        /// <param name="user">The user entity containing claims data.</param>
        /// <returns>A signed JWT string.</returns>
        string CreateAccessToken(User user);

        /// <summary>
        /// Creates a unique, random string used as a Refresh Token.
        /// </summary>
        /// <param name="user">The user entity.</param>
        /// <returns>A Base64 encoded random string.</returns>
        string CreateRefreshToken(User user);

        /// <summary>
        /// Attempts to extract claims principal from an expired JWT without full security validation.
        /// Used primarily in refresh token flows (future complexity).
        /// </summary>
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
