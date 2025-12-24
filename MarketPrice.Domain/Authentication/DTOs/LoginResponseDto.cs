using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Domain.Authentication.DTOs
{
    public class LoginResponseDto
    {
        public  string FirstName { get; set; } = string.Empty;
        public string FamilyName { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public bool success { get; set; }
        public string LoginStatus { get; set; } = string.Empty;
        public DateTimeOffset ExpiryDate { get; set; }

    }
}
