using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Data.Models
{
    public class VerificationOtp
    {   
         public Guid VerificationOtpId { get; set; }

        public Guid VerificationId { get; set; }

        public string CodeHash { get; set; } = string.Empty;

        public string Destination { get; set; } = string.Empty;

        public string VerificationMethod { get; set; } = string.Empty;

        public DateTime DateCreated { get; set; }

        public DateTime DateExpires { get; set; }

        public DateTime? DateUsed { get; set; }

        public int Attempts { get; set; }

        public int MaxAttempts { get; set; } = 5;

        public bool IsUsed { get; set; }

        public bool IsInvalidated { get; set; }

    }
}
