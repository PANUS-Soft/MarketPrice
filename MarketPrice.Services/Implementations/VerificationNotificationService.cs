using MarketPrice.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace MarketPrice.Services.Implementations;

public class VerificationNotificationService
    : IVerificationNotificationService
{
    private readonly ILogger<VerificationNotificationService> _logger;

    public VerificationNotificationService(
        ILogger<VerificationNotificationService> logger)
    {
        _logger = logger;
    }

    public Task SendAsync(
        string method,
        string destination,
        string code,
        CancellationToken cancellationToken = default)
    {
        // DEVELOPMENT ONLY.
        // Later replace this with real SMS/email providers.

        _logger.LogInformation(
            "OTP [{Method}] sent to {Destination}: {Code}",
            method,
            destination,
            code);

        return Task.CompletedTask;
    }
}