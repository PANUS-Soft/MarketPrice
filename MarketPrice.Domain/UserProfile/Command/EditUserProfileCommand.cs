using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Domain.UserProfile.Command
{
    public class EditUserProfileCommand
    {
        public string FirstName { get; set; }
        public string FamilyName { get; set; }
        public string? OtherName { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
    }
}
