using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Domain.Profile.Commands
{
    public class UpdateUserProfileCommand
    {
        public Guid UserId { get; set; }
        public string? FirstName { get; set; }
        public string? FamilyName { get; set; }
        public string? OtherNames { get; set; }
        public string? Bio { get; set; }
        public string? EmailAddress { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
