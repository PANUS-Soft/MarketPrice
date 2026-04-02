using MarketPrice.Domain.Profile.Commands;
using MarketPrice.Domain.Profile.DTOs;

namespace MarketPrice.Services.Interfaces
{
    public interface IChangePasswordService
    {
        Task<ChangePasswordResponseDto> ChangePasswordAsync(ChangePasswordCommand command);
    }
}
