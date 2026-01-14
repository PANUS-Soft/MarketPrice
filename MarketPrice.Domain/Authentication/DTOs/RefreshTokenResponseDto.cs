using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Domain.Authentication.DTOs
{
    public class RefreshTokenResponseDto : AuthenticationResponseDto
    {
        public string? Message { get; init; }
    }
}
