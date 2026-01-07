using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Domain.Authentication.Commands
{
    public class RefreshTokenCommand
    {
        public required string RefreshToken { get; set; }
    }
}
