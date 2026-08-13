using MarketPrice.Domain.Profile.Commands;
using MarketPrice.Domain.Profile.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Services.Interfaces
{
    public interface IVerificationService
    {
        Task<SendVerificationCodeResponseDto> SendCodeAsync(Guid userId,SendVerificationCodeCommand command, CancellationToken cancellationToken = default);

        Task<VerifyVerificationCodeResponseDto> VerifyCodeAsync(Guid userId, VerifyVerificationCodeCommand command, CancellationToken cancellationToken = default);
    }
}
    