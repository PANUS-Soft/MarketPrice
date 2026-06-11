using MarketPrice.Domain.Activity.DTOs;
using MarketPrice.Domain.Authentication.DTOs;
using MarketPrice.Domain.Position.Commands;
using MarketPrice.Domain.Position.DTOs;

namespace MarketPrice.Services.Interfaces
{
    public interface IPositionService
    {
        Task<PositionListingResponseDto> GetPositionListingsAsync(PositionListingCommand command);
        Task<UpdatePositionResponseDto> UpdatePositionAsync(UpdatePositionCommand command, bool isOffer);
        Task<PositionResponseDto> ProcessPositionAsync(CreatePositionCommand command, bool isOffer);
        Task<ActivityGroupDto> GetActivityAsync(Guid userId);
        Task<DeleteActivityResponseDto> DeleteActivityAsync(Guid positionId);
        Task<PositionDetailResponseDto> GetPositionDetailAsync(Guid id);
    }
}