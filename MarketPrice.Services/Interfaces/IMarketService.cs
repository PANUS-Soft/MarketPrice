using MarketPrice.Domain.Market.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarketPrice.Domain.Market.Commands;


namespace MarketPrice.Services.Interfaces
{
    public interface IMarketService
    {
        Task<List<MarketInsightResponseDto>> GetMarketTrendAsync(MarketInsightCommand command);
    }
}
