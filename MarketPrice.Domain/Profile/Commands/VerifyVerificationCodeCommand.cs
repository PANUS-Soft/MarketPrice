using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Domain.Profile.Commands
{
    public class VerifyVerificationCodeCommand
    {
       public Guid VerificationId { get; set; }
       public string VerificationCode { get; set; } = string.Empty;



    }
}
