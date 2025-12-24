using MarketPrice.Domain.Authentication.Commands;
using MarketPrice.Domain.Authentication.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Services.Interfaces
{
    public interface ILogoutService
    {
        /// <summary>
        /// Handles session close up of a user
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        Task<LogoutResponseDto> LogoutAsync(LogoutCommand command);

    }
}
