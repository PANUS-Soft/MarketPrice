using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarketPrice.Domain.Authentication.DTOs;

namespace MarketPrice.Domain.Authentication
{
    public class AuthenticationResponseDto : BaseResponseDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime ExpiryDate { get; set; }
        public Guid UserId { get; set; }
    }
}
