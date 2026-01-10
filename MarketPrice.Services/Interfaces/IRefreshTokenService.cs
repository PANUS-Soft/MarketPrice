using MarketPrice.Domain.Authentication.Commands;
using MarketPrice.Domain.Authentication.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Services.Interfaces
{
    public interface IRefreshTokenService
    {
        Task<RefreshTokenResponseDto> RefreshTokenAsync(RefreshTokenCommand command);
    }
}
