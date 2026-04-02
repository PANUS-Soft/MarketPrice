using MarketPrice.Domain.Market.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Services.Interfaces
{
    public interface IPriceService
    {
        Task<List<MarketInsightChartResponseDto>> GetPriceAsync(Guid commodityId, string interval, DateTime? from, DateTime? to);
    }
}
