using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Domain.Authentication.DTOs
{
    public class LoginResponseDto
    {
        public string? Username { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string FamilyName { get; set; } = string.Empty;

        public bool RememberMe { get; set; }

        public bool IsAuthenticated { get; set; } // did login request suceed

    }
}
