using LinqToDB.Async;
using MarketPrice.Data;
using MarketPrice.Data.Models;
using MarketPrice.Domain.Profile.Commands;
using MarketPrice.Domain.Profile.DTOs;
using MarketPrice.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Services.Implementations
{
    public class VerificationService : IVerificationService
    {
        private readonly MarketPriceDbContext _context;
        private readonly IOtpService _otpService;
        private readonly IVerificationNotificationService _notificationService;
        private readonly IConfiguration _configuration;

        public VerificationService( MarketPriceDbContext context,IOtpService otpService, IVerificationNotificationService notificationService, IConfiguration configuration)
        {
            _context = context;
            _otpService = otpService;
            _notificationService = notificationService;
            _configuration = configuration;
        }


        //Send the verification code to the user 
        public async Task<SendVerificationCodeResponseDto> SendCodeAsync(Guid userId, SendVerificationCodeCommand command, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(command.Destination))
            {
                throw new ArgumentException(
                    "Phone number or email is required.");
            }

            var method = command.VerificationMethod
                .Trim()
                .ToLowerInvariant();

            if (method != "phone" && method != "email")
            {
                throw new ArgumentException(
                    "Verification method must be phone or email.");
            }

            // Make sure the user exists.
            var userExists = await _context.Users.AnyAsync(x => x.UserId == userId, cancellationToken);

            if (!userExists)
            {
                throw new KeyNotFoundException(
                    "User was not found.");
            }

            // Invalidate previous OTPs for this user's active
            // verification process.
            var previousOtps = await _context.VerificationOtps
      .Where(x =>
          !x.IsUsed &&
          !x.IsInvalidated &&
          x.DateExpires > DateTimeOffset.UtcNow)
      .ToListAsync(cancellationToken);

            foreach (var oldOtp in previousOtps)
            {
                oldOtp.IsInvalidated = true;
            }

            // IMPORTANT:
            // Replace this section with the constructor/property
            // names of your existing Verification entity.

            var verification = new Verification
            {
                UserId = userId,

                // We will connect the actual lookup ID here
                // after checking your LookupData values.
                VerificationTypeId = 0,

                // We will connect the actual Pending status ID here.
                CurrentVerificationStatusId = 0,

                DateStarted = DateTime.UtcNow,
                Notes = $"Verification initiated using {method}."
            };

            _context.Verifications.Add(verification);

            await _context.SaveChangesAsync(cancellationToken);

            // Generate OTP.
            var code = _otpService.GenerateCode();

            var hash = _otpService.HashCode(code);

            var expirationMinutes =
                _configuration.GetValue<int?>(
                    "Otp:ExpirationMinutes") ?? 5;

            var maxAttempts =
                _configuration.GetValue<int?>(
                    "Otp:MaxAttempts") ?? 5;

            var otp = new VerificationOtp
            {
                VerificationId = verification.VerificationId,
                Destination = command.Destination.Trim(),
                VerificationMethod = method,
                CodeHash = hash,
                DateCreated = DateTime.UtcNow,
                DateExpires = DateTime.UtcNow.AddMinutes(
                    expirationMinutes),
                Attempts = 0,
                MaxAttempts = maxAttempts,
                IsUsed = false,
                IsInvalidated = false
            };

            _context.VerificationOtps.Add(otp);

            await _context.SaveChangesAsync(cancellationToken);

            await _notificationService.SendAsync(
                method,
                command.Destination.Trim(),
                code,
                cancellationToken);

            return new SendVerificationCodeResponseDto
            {
                Success = true,
                VerificationId = verification.VerificationId,
                Message = $"Verification code sent via {method}."
            };
        }

        // When the code is revices we have now to check if i can be verify.
        public async Task<VerifyVerificationCodeResponseDto> VerifyCodeAsync(Guid userId, VerifyVerificationCodeCommand command, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(command.VerificationCode))
            {
                return new VerifyVerificationCodeResponseDto
                {
                    Success = false,
                    Message = "Verification code is required."
                };
            }

            var verification = await _context.Verifications.FirstOrDefaultAsync(x => x.VerificationId == command.VerificationId && x.UserId == userId, cancellationToken);

            if (verification == null)
            {
                return new VerifyVerificationCodeResponseDto
                {
                    Success = false,
                    Message = "Verification request was not found."
                };
            }

            var otp = await _context.VerificationOtps
                .Where(x =>
                    x.VerificationId ==
                    command.VerificationId &&
                    !x.IsInvalidated)
                .OrderByDescending(x => x.DateCreated)
                .FirstOrDefaultAsync(cancellationToken);

            if (otp == null)
            {
                return new VerifyVerificationCodeResponseDto
                {
                    Success = false,
                    Message = "No active verification code was found."
                };
            }

            if (otp.IsUsed)
            {
                return new VerifyVerificationCodeResponseDto
                {
                    Success = false,
                    Message = "This verification code has already been used."
                };
            }

            if (DateTime.UtcNow > otp.DateExpires)
            {
                otp.IsInvalidated = true;

                await _context.SaveChangesAsync(
                    cancellationToken);

                return new VerifyVerificationCodeResponseDto
                {
                    Success = false,
                    Message = "Verification code has expired."
                };
            }

            if (otp.Attempts >= otp.MaxAttempts)
            {
                otp.IsInvalidated = true;

                await _context.SaveChangesAsync(
                    cancellationToken);

                return new VerifyVerificationCodeResponseDto
                {
                    Success = false,
                    Message = "Too many verification attempts."
                };
            }

            otp.Attempts++;

            var valid = _otpService.VerifyCode(
                command.VerificationCode.Trim(),
                otp.CodeHash);

            if (!valid)
            {
                if (otp.Attempts >= otp.MaxAttempts)
                {
                    otp.IsInvalidated = true;
                }

                await _context.SaveChangesAsync(
                    cancellationToken);

                return new VerifyVerificationCodeResponseDto
                {
                    Success = false,
                    Message = "Invalid verification code."
                };
            }

            // OTP is correct.
            otp.IsUsed = true;
            otp.DateUsed = DateTime.UtcNow;

            // Complete the existing Verification.
            verification.DateCompleted = DateTime.UtcNow;

            // IMPORTANT:
            // Set the actual Completed status ID here
            // once we inspect your LookupData.

            verification.CurrentVerificationStatusId = 0;

            await _context.SaveChangesAsync(
                cancellationToken);

            return new VerifyVerificationCodeResponseDto
            {
                Success = true,
                Message = "Identity verified successfully."
            };
        }
    }
}
