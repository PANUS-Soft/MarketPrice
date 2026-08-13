using System.Security.Cryptography;
using System.Text;
using MarketPrice.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace MarketPrice.Services.Services;

public class OtpService : IOtpService
{
    private readonly IConfiguration _configuration;

    public OtpService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateCode()
    {
        return RandomNumberGenerator
            .GetInt32(100000, 1000000)
            .ToString();
    }

    public string HashCode(string code)
    {
        var secret = _configuration["Otp:Secret"];

        if (string.IsNullOrWhiteSpace(secret))
        {
            throw new InvalidOperationException(
                "Otp:Secret is not configured.");
        }

        using var hmac = new HMACSHA256(
            Encoding.UTF8.GetBytes(secret));

        var hash = hmac.ComputeHash(
            Encoding.UTF8.GetBytes(code));

        return Convert.ToHexString(hash);
    }

    public bool VerifyCode(
        string code,
        string storedHash)
    {
        var calculatedHash = HashCode(code);

        return CryptographicOperations.FixedTimeEquals(
            Convert.FromHexString(calculatedHash),
            Convert.FromHexString(storedHash));
    }
}