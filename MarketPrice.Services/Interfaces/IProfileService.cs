using MarketPrice.Domain.Profile.DTOs;

namespace MarketPrice.Services.Interfaces;

public interface IProfileService
{
    Task<UserProfileResponseDto> GetUserProfileAsync(Guid id);
}