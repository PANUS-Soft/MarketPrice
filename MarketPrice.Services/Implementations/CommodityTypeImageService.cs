using MarketPrice.Data;
using MarketPrice.Domain.CommodityTypeImage.Commands;
using MarketPrice.Domain.CommodityTypeImage.Dtos;
using MarketPrice.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MarketPrice.Services.Implementations
{
    public class CommodityTypeImageService(MarketPriceDbContext context):ICommodityTypeImageService
    {
        private readonly MarketPriceDbContext _context = context;

        public async Task<CommodityTypeImageResponseDto> GetCommodityTypeImageAsync(CommodityTypeImageCommand command)
        {
            var image = await _context.CommodityTypeImage
                .Where(x => x.CommodityTypeId == command.CommodityTypeId)
                .Select(x => new CommodityTypeImageResponseDto
                {
                    ImageData = x.ImageData,
                    ContentType = x.ContentType
                })
                .FirstOrDefaultAsync();

            return image;
        }
    }
}
