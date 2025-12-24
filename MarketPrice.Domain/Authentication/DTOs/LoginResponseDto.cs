using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Domain.Authentication.DTOs
{
    public class LoginResponseDto
    {
        public  string FirstName { get; set; } 
        public string FamilyName { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public bool LoginStatus { get; set; }
        public DateTimeOffset ExpiryDate { get; set; }

    }
}
