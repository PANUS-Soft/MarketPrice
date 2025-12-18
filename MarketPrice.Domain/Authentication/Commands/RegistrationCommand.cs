using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Domain.Authentication.Commands
{
    public class RegistrationCommand
    {
        public required string FirstName { get; set; }
        public required string FamilyName { get; set; }
        public string?  OtherNames { get; set; }
        public required int AccountTypeId { get; set; }
        public required string EmailAddress { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Password { get; set; }

    }



}
