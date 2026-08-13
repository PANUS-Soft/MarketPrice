using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Domain.Profile.DTOs
{
    public class SendVerificationCodeResponseDto
    {
        public bool Success { get; set; }
        public Guid VerificationId { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
