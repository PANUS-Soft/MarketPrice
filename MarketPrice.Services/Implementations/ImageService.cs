using MarketPrice.Domain.Image.DTOs;
using MarketPrice.Services.Interfaces;
using LinqToDB.Async;
using MarketPrice.Data;

namespace MarketPrice.Services.Implementations
{
    public class ImageService (MarketPriceDbContext context) : IImageService
    {
        public async Task<ImageResponseDto> GetCommodityTypeImageAsync(Guid id)
        {
            var image = await context.CommodityTypeImage
                .Where(x => x.CommodityTypeId == id)
                .Select(x => new ImageResponseDto
                {
                    ImageData = x.ImageData,
                    ContentType = x.ContentType
                }).FirstOrDefaultAsync();

            if (image == null) return null;

            return image;
        }

        public async Task<ImageResponseDto> GetCommodityImageAsync(Guid id)
        {
            var image = await context.CommodityImage
                .Where(x => x.CommodityId == id)
                .Select(x => new ImageResponseDto
                {
                    ImageData = x.ImageData,
                    ContentType = x.ContentType
                }).FirstOrDefaultAsync();

            if (image == null) return null;

            return image;
        }
    }
}
