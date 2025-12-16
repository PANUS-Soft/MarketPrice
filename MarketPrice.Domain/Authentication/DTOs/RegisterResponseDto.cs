using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Domain.Authentication.DTOs
{
    public class RegisterResponseDto
    {

        public string EmailAddress { get; set; } = string.Empty;
        public string CreationStatus { get; set; } = string.Empty;
    }
}
