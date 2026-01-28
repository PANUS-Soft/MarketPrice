using MarketPrice.Domain.Market.DTOs;


namespace MarketPrice.Services.Interfaces
{
    public interface IMarketService
    {
        Task<List<MarketResponseDto>> GetMarketTrendAsync();
        Task<MarketInsightResponseDto> GetMarketInsightAsync(Guid commodityId);
    }
}
