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

        public DateTime LastActivityDate { get; set; } = DateTime.UtcNow;
    }
}
