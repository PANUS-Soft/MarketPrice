using MarketPrice.Domain.Image.DTOs;

namespace MarketPrice.Services.Interfaces
{
    public interface IImageService
    {
        Task<ImageResponseDto> GetCommodityTypeImageAsync(Guid id);

        Task<ImageResponseDto> GetCommodityImageAsync(Guid id);
    }
}
