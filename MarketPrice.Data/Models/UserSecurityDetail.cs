using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Data.Models
{
    public class UserSecurityDetail
    {
        public Guid SecurityId { get; set; } 
        public required Guid UserId { get; set; }

        public string? RefreshToken { get; set; }

        public DateTime? RefreshTokenExpiryTime { get; set; }


        // Have to update DateTime.UtcNow to DateTimeOffset.UtcNow for global time (GMT) and
        // offset (Difference between the local time and standard global time.)
        public DateTime LastActivityDate { get; set; } = DateTime.UtcNow;
    }
}
