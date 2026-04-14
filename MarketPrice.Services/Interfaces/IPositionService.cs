using MarketPrice.Domain.Activity.Command;
using MarketPrice.Domain.Activity.Dto;
using MarketPrice.Domain.Position.Commands;
using MarketPrice.Domain.Position.DTOs;

namespace MarketPrice.Services.Interfaces
{
    public interface IPositionService
    {
        Task<PositionListingResponseDto> GetPositionListingsAsync(PositionListingCommand command);
        Task<PositionResponseDto> ProcessPositionAsync(PositionCommand command, bool isOffer);
        Task<ActivityGroupDto> GetActivityAsync(ActivityCommand command);
        Task<PositionDetailResponseDto> GetPositionDetailAsync(Guid id);

    }
}