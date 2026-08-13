using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Domain.Profile.DTOs
{
    public class VerifyVerificationCodeResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;

     
    }
}
