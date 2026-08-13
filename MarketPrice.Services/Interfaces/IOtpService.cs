using MarketPrice.Domain.Profile.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarketPrice.Domain.Profile.Commands;
namespace MarketPrice.Services.Interfaces
{
    public interface IOtpService
    {
        string GenerateCode();

        string HashCode(string code);

        bool VerifyCode(string code, string storedHash);
    }
}
    