using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Domain.Profile.Commands
{
    public class SendVerificationCodeCommand
    {
        public string VerificationMethod { get; set; } = string.Empty; // this where the user chose phoneNumber or Email
        public string Destination {  get; set; } = string.Empty; // the actual phone number or the Email 
    }
}
