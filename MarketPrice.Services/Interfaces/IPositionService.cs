using MarketPrice.Domain.Position.Commands;
using MarketPrice.Domain.Position.DTOs;

namespace MarketPrice.Services.Interfaces
{
    public interface IPositionService
    {
        Task<PositionResponseDto> ProcessPositionAsync(PositionCommand command, bool isOffer);

        Task<PositionListingResponseDto> GetPositionListingsAsync(PositionListingCommand command);
        
        Task<PositionDetailResponseDto> GetPositionDetailAsync(Guid id);

    }
}