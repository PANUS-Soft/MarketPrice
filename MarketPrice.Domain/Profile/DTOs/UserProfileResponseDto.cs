using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarketPrice.Domain.Authentication.DTOs;

namespace MarketPrice.Domain.Profile.DTOs
{
    public class UserProfileResponseDto : BaseResponseDto
    {
        public Guid UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string FamilyName { get; set; } = string.Empty;
        public string OtherName { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string AccountType { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
    }
}
