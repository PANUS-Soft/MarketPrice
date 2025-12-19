using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Domain.Authentication.Commands
{
    public class LogoutCommand
    {
        public required string EmailAddress { get; set; }
    }
}
