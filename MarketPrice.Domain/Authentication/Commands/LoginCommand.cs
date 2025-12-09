using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Domain.Authentication.Commands
{
    public class LoginCommand
    {
        public DateTime LoginDate { get; set; }
        public required string Password { get; set; }
        public required string Username { get; set; }
        public bool RememberMe { get; set; }

    }
}
