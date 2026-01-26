using MarketPrice.Domain.CommodityTypeImage.Commands;
using MarketPrice.Domain.CommodityTypeImage.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Services.Interfaces
{
    public interface ICommodityTypeImageService
    {
        Task<CommodityTypeImageResponseDto> GetCommodityTypeImageAsync(CommodityTypeImageCommand command);
    }
}
