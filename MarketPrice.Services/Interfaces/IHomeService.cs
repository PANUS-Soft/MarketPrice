using MarketPrice.Domain.Home.DTOs;

namespace MarketPrice.Services.Interfaces
{
    public interface IHomeService
    {
        Task<List<LoadHomeResponseDto>> LoadHomeAsync();
    }
}
